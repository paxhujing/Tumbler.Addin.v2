using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 用于WPF的插件配置解析器。
    /// </summary>
    public class WpfAddinConfigParser : AddinConfigParser
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.WpfAddinConfigParser 实例。
        /// </summary>
        /// <param name="configFile">配置文件。</param>
        public WpfAddinConfigParser(String configFile) 
            : base(configFile)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 获取插件信息。
        /// </summary>
        /// <param name="root">插件配置。</param>
        /// <param name="location">配置文件相对路径。</param>
        /// <returns>插件信息。</returns>
        protected override AddinInfo GetAddinInfoImpl(XElement root, string location)
        {
            return new WpfAddinInfo(root, location);
        }

        /// <summary>
        /// 获取插件架构。
        /// </summary>
        /// <returns>插件架构。</returns>
        protected override Stream GetAddinSchemaStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Tumbler.Addin.Wpf.AddinConfigSchema.xsd");
        }

        #endregion
    }
}
