using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件代理。用于插件与宿主之间的通信。
    /// </summary>
    public abstract class AddinProxy : MarshalByRefObject
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
            MessageDispather = new MessageDispathcer(target);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 所关联插件的ID号。
        /// </summary>
        public String Id { get; }

        /// <summary>
        /// 消息服务。
        /// </summary>
        internal MessageService MessageService { get; set; }

        /// <summary>
        /// 消息调度器。
        /// </summary>
        internal MessageDispathcer MessageDispather { get;  }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 创建插件。
        /// </summary>
        /// <param name="proxy">与插件关联的代理。</param>
        /// <returns>插件。</returns>
        protected abstract IAddin CreateAddin(AddinProxy proxy);

        #endregion

        #region Internal

        /// <summary>
        /// 将消息发送给消息中心，让其调度。
        /// </summary>
        /// <param name="message">消息。</param>
        internal void Send(Message message)
        {
            MessageService?.OnReceive(message);
        }

        #endregion

        #endregion
    }
}
