using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// WPF的插件信息。
    /// </summary>
    public class WpfAddinInfo : AddinInfo
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.WpfAddinInfo 实例。
        /// </summary>
        /// <param name="root">插件配置。</param>
        /// <param name="location">配置文件相对路径。</param>
        internal WpfAddinInfo(XElement root, String location)
            : base(root, location)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 激活器的类型限定名。
        /// </summary>
        public String ActivatorType { get; internal set; }

        #endregion

        #region Methods

        #region Protected

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="root">插件配置。</param>
        /// <param name="location">配置文件相对路径。</param>
        protected override void Initialize(XElement root, String location)
        {
            base.Initialize(root, location);
            ActivatorType = root.Attribute("activatorType")?.Value;
        }

        #endregion

        #endregion
    }
}
