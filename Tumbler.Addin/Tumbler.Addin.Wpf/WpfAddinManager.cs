using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 插件管理器。
    /// </summary>
    public class WpfAddinManager : AddinManager
    {
        #region Fields

        private WpfAddinLoader _loader;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.AddinManager 实例。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="globalConfigFile">全局配置文件。</param>
        public WpfAddinManager(IAddinHost host, string globalConfigFile)
            : base(host, globalConfigFile)
        {
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取插件的激活器。
        /// </summary>
        /// <param name="info">插件信息。</param>
        /// <returns>插件激活器。</returns>
        public AddinActivatorBase GetAddinActivator(WpfAddinInfo info)
        {
            String privateBinPath = $"addins/{System.IO.Path.GetDirectoryName(info.Location)}";
            AddPrivateBinPath(privateBinPath);
            AddinActivatorBase activator = _loader.GetAddinActivator(info);
            if (activator != null)
            {
                activator._info = info;
                activator._addinManager = this;
            }
            return activator;
        }

        #endregion

        #region Protected

        /// <summary>
        /// 创建插件解析器。
        /// </summary>
        /// <param name="globalConfigFile">插件全局配置文件。</param>
        /// <returns>插件解析器。</returns>
        protected override AddinConfigParser CreateAddinConfigParser(String globalConfigFile)
        {
            return new WpfAddinConfigParser(globalConfigFile);
        }

        /// <summary>
        /// 创建插件加载器。
        /// </summary>
        /// <returns>插件加载器。</returns>
        protected override AddinLoader CreateAddinLoader()
        {
            _loader = new WpfAddinLoader();
            return _loader;
        }

        #endregion

        #endregion
    }
}
