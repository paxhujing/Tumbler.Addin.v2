﻿using System;
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
            if (target.MessageDispatcher == null) throw new ArgumentNullException("target.MessageDispatcher");
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
        /// 将消息发送给消息中心让其调度。
        /// </summary>
        /// <param name="message">消息。</param>
        public void Send(Message message)
        {
            MessageService?.Transmit(message);
        }

        /// <summary>
        /// 将消息转发给实际的对象。
        /// </summary>
        /// <param name="message">消息。</param>
        public void OnReceive(Message message)
        {
            MessageDispatcher.Queue(message);
        }

        #endregion

        #endregion
    }
}
