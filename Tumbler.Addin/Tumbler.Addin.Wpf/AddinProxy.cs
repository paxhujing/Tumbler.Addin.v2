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
    public class AddinProxy : Core.AddinProxy
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.AddinProxy 实例。
        /// </summary>
        /// <param name="target">插件。</param>
        /// <param name="uiType">UI元素的类型限定名。</param>
        protected AddinProxy(IAddin target, String uiType)
            : base(target)
        {
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

        #endregion
    }
}
