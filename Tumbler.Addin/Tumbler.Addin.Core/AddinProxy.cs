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
    public abstract class AddinProxy : MarshalByRefObject, IMessageSource, IMessageTarget
    {
        #region Fields

        /// <summary>
        /// 与该代理关联的插件。
        /// </summary>
        private readonly IAddin _target;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.Delegator 实例。
        /// </summary>
        /// <param name="target">代表的插件。</param>
        protected AddinProxy()
        {
            IAddin target = CreateAddin(this);
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

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 将消息发送给消息中心让其调度。
        /// </summary>
        /// <param name="message">消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Send(Message message)
        {
            MessageService?.Transmit(message);
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
        public virtual void Load() { }

        /// <summary>
        /// 当插件实例被卸载时调用该方法。
        /// </summary>
        public virtual void Unload() { }

        /// <summary>
        /// 在租约管理器中，该对象永不过期，必须显示移除其根。
        /// </summary>
        /// <returns>对象租约。</returns>
        public override Object InitializeLifetimeService()
        {
            ILease lease = (ILease)base.InitializeLifetimeService();
            lease.InitialLeaseTime = TimeSpan.FromMilliseconds(0);
            return lease;
        }

        #endregion

        #region Protected

        /// <summary>
        /// 创建插件。
        /// </summary>
        /// <param name="proxy">与插件关联的代理。</param>
        /// <returns>插件。</returns>
        protected abstract IAddin CreateAddin(AddinProxy proxy);

        #endregion

        #endregion
    }
}
