using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 内部消息收发器。
    /// </summary>
    public sealed class InternalMessageTransceiver : MarshalByRefObject
    {
        #region Event

        /// <summary>
        /// 收到内部消息。
        /// </summary>
        public event EventHandler<InternalMessage> ReceiveInternalMessage;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.InternalMessageListner 实例。
        /// </summary>
        /// <param name="proxy">插件代理。</param>
        public InternalMessageTransceiver(WpfAddinProxy proxy)
        {
            Proxy = proxy;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 插件代理。
        /// </summary>
        internal WpfAddinProxy Proxy { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 发送内部消息。
        /// </summary>
        /// <param name="messageCode">消息码。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns></returns>
        public InternalMessage SendInternalMessage(Int32 messageCode, Core.ContentType contentType, Object content)
        {
            InternalMessage message = new InternalMessage(messageCode, contentType, content);
            Proxy.OnInternalReceive(message);
            return message;
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

        #endregion

        #region Internal

        /// <summary>
        /// 将来自代理的消息以事件方式转发给UI。
        /// </summary>
        /// <param name="message">内部消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal void TransmitInternalMessage(InternalMessage message)
        {
            OnReceiveInternalMessage(message);
        }

        #endregion

        #region Private

        /// <summary>
        /// 出发内部消息接收事件。
        /// </summary>
        /// <param name="message">内部消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void OnReceiveInternalMessage(InternalMessage message)
        {
            if (ReceiveInternalMessage != null)
            {
                ReceiveInternalMessage(this, message);
            }
        }

        #endregion

        #endregion

    }
}
