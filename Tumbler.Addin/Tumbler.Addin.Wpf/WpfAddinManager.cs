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
    public class WpfAddinManager : Core.AddinManager
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
        /// 获取插件的UI元素。
        /// </summary>
        /// <param name="proxy">插件代理。</param>
        /// <returns>UI元素。</returns>
        public FrameworkElement GetAddinUI(WpfAddinProxy proxy)
        {
            return _loader.GetAddinUI(proxy);
        }

        /// <summary>
        /// 卸载有UI的插件。
        /// </summary>
        /// <param name="ui">UI元素。</param>
        public void Unload(FrameworkElement ui)
        {
            _loader.Unload(ui);
        }

        /// <summary>
        /// 卸载有UI的插件。
        /// </summary>
        /// <param name="uis">一组UI元素。</param>
        public void Unload(IEnumerable<FrameworkElement> uis)
        {
            foreach (FrameworkElement ui in uis)
            {
                _loader.Unload(ui);
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// 获取插件加载器。
        /// </summary>
        /// <returns>插件加载器。</returns>
        protected override Core.AddinLoader CreateAddinLoader()
        {
            _loader = new WpfAddinLoader();
            return _loader;
        }

        #endregion

        #endregion
    }
}
