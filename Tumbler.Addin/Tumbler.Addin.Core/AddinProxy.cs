using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件代理。用于插件与宿主之间的通信。
    /// </summary>
    public class AddinProxy : MarshalByRefObject, IAddin
    {
        #region Fields

        /// <summary>
        /// 与该代理关联的插件。
        /// </summary>
        private readonly IAddin _target;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinProxy 实例。
        /// </summary>
        /// <param name="target">插件。</param>
        protected AddinProxy(IAddin target)
        {
            if (target == null) throw new ArgumentNullException("target");
            if (String.IsNullOrWhiteSpace(target.Id)
                || target.Id == MessageService.AddinHostId
                || target.Id == MessageService.AllTargetsId)
            {
                throw new ArgumentException("Invalid addin id");
            }
            _target = target;
            Id = target.Id;
            MessageDispatcher = target.MessageDispatcher;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 所关联插件的Id号。
        /// </summary>
        public String Id { get; }

        /// <summary>
        /// 消息调度器。
        /// </summary>
        public MessageDispatcher MessageDispatcher { get; }

        /// <summary>
        /// 消息服务。
        /// </summary>
        internal MessageService MessageService { get; set; }

        /// <summary>
        /// 拥有此代理的应用程序域。
        /// </summary>
        internal AppDomain Owner { get; set; }

        /// <summary>
        /// 代理关联的对象。
        /// </summary>
        protected IMessageTarget Target => _target;

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取宿主的真实Id。
        /// </summary>
        /// <returns>宿主的真实Id。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public String GetHostActualId()
        {
            return MessageService?.GetHostActualId();
        }

        /// <summary>
        /// 将消息转发给实际的对象。
        /// </summary>
        /// <param name="message">消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void OnReceive(Message message)
        {
            MessageDispatcher?.Queue(message);
        }

        /// <summary>
        /// 当插件实例被创建时调用该方法。
        /// </summary>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Initialize()
        {
            _target.Initialize();
        }

        /// <summary>
        /// 当插件实例被卸载时调用该方法。
        /// </summary>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Destroy()
        {
            _target.Destroy();
        }

        /// <summary>
        /// 在租约管理器中，该对象永不过期，必须显示移除其根。
        /// </summary>
        /// <returns>对象租约。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public override Object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            lease.InitialLeaseTime = TimeSpan.FromMilliseconds(0);
            return lease;
        }

        /// <summary>
        /// 获取对象的字符串表示。
        /// </summary>
        /// <returns>对象的字符串表示。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public override String ToString()
        {
            return Id;
        }

        #endregion

        #region Internal

        /// <summary>
        /// 将消息发送给消息中心让其调度。
        /// </summary>
        /// <param name="message">消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal void Send(Message message)
        {
            MessageService?.Transmit(message);
        }

        #endregion

        #endregion
    }
}
