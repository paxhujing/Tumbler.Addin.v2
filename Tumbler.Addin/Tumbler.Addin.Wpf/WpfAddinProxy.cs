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
    public class WpfAddinProxy : AddinProxy
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.AddinProxy 实例。
        /// </summary>
        /// <param name="target">插件。</param>
        /// <param name="uiType">UI元素的类型限定名。</param>
        protected WpfAddinProxy(IWpfAddin target, String uiType)
            : base(target)
        {
            if (String.IsNullOrWhiteSpace(uiType)) throw new ArgumentNullException("uiType");
            UIType = uiType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 所在目录。
        /// </summary>
        internal String Directory { get; set; }

        /// <summary>
        /// UI元素的类型限定名。
        /// </summary>
        public String UIType { get; }

        /// <summary>
        /// 内部事件侦听器。
        /// </summary>
        internal InternalMessageListener Listener { get; set; }

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
            Listener?.TransmitInternalMessage(message);
        }

        #endregion

        #endregion
    }
}
