using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Schema;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 插件加载器。
    /// </summary>
    public class WpfAddinLoader : AddinLoader
    {
        #region Methods

        #region Public

        /// <summary>
        /// 获取插件的UI元素。
        /// </summary>
        /// <param name="proxy">插件代理。</param>
        /// <returns>UI元素。</returns>
        public FrameworkElement GetAddinUI(WpfAddinProxy proxy)
        {
            Type uiType = ParseType(proxy.UIType, proxy.Directory);
            if (uiType == null) return null;
            try
            {
                Assembly assembly = AppDomain.CurrentDomain.Load(uiType.Assembly.FullName);
                FrameworkElement ui = assembly.CreateInstance(uiType.FullName) as FrameworkElement;
                if (ui != null)
                {
                    InternalMessageListener listener = new InternalMessageListener(proxy);
                    proxy.Listener = listener;
                    ui.Tag = listener;
#if DEBUG
                    Console.WriteLine($"Created addin {proxy.Id} ui elemtnt");
#endif
                }
                return ui;
            }
            catch (MissingMemberException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 卸载有UI的插件。
        /// </summary>
        /// <param name="ui">UI元素。</param>
        public void Unload(FrameworkElement ui)
        {
            InternalMessageListener listener = ui.Tag as InternalMessageListener;
            if (listener == null) return;
            ui.Tag = null;
            base.Unload(listener.Proxy);
        }

        #endregion

        #region Protected

        /// <summary>
        /// 将插件加载到独立的应用程序域中。
        /// </summary>
        /// <param name="addinType">实现了 IAddin 接口的类型。</param>
        /// <param name="doc">配置。</param>
        /// <returns>派生自 AddinProxy 类的实例。</returns>
        protected override IMessageTarget LoadOnIsolatedAppDomain(Type addinType, XDocument doc)
        {
            AddinProxy proxy = base.LoadOnIsolatedAppDomain(addinType, doc) as AddinProxy;
            if (proxy != null)
            {
                WpfAddinProxy wpfProxy = proxy as WpfAddinProxy;
                if (wpfProxy != null) wpfProxy.Directory = Path.GetDirectoryName(addinType.Assembly.Location);
            }
            return proxy;
        }

        #endregion

        #endregion
    }
}
