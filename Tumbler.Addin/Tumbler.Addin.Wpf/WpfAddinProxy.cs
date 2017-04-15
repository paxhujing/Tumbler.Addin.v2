using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 插件代理。用于插件与宿主之间的通信。
    /// </summary>
    public class WpfAddinProxy : AddinProxy,IWpfAddin
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.AddinProxy 实例。
        /// </summary>
        /// <param name="target">插件。</param>
        protected WpfAddinProxy(IWpfAddin target)
            : base(target)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 所在目录。
        /// </summary>
        internal String Directory { get; set; }

        /// <summary>
        /// 内部消息收发器。
        /// </summary>
        internal InternalMessageTransceiver Transceiver { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 内部消息请求。
        /// </summary>
        /// <param name="message">内部消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void OnInternalReceive(InternalMessage message)
        {
            ((IWpfAddin)Target).OnInternalReceive(message);
        }

        #endregion

        #region Protected

        /// <summary>
        /// 出发内部消息接收事件。
        /// </summary>
        /// <param name="message">内部消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal void OnSendInternalMessage(InternalMessage message)
        {
            Transceiver?.TransmitInternalMessage(message);
        }

        #endregion

        #endregion
    }
}
