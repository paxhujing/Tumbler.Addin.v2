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
        /// 获取插件的激活器。
        /// </summary>
        /// <param name="addinInfo">插件信息。</param>
        /// <returns>插件激活器。</returns>
        public AddinActivatorBase GetAddinActivator(WpfAddinInfo addinInfo)
        {
            Type activatorType = ParseType(addinInfo.ActivatorType, addinInfo.Directory);
            if (activatorType == null || activatorType.BaseType.AssemblyQualifiedName != typeof(AddinActivatorBase).AssemblyQualifiedName)
            {
                return null;
            }
            try
            {
                Assembly assembly = AppDomain.CurrentDomain.Load(activatorType.Assembly.FullName);
                AddinActivatorBase activator = (AddinActivatorBase)assembly.CreateInstance(activatorType.FullName);
#if DEBUG
                if (activator != null)
                {
                    Console.WriteLine($"Created activator {activatorType.Assembly.FullName}");
                }
#endif
                return activator;
            }
            catch (MissingMemberException ex)
            {
                return null;
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// 将插件加载到独立的应用程序域中。
        /// </summary>
        /// <param name="addinType">实现了 IAddin 接口的类型。</param>
        /// <param name="info">插件配置信息。</param>
        /// <returns>派生自 AddinProxy 类的实例。</returns>
        protected override IAddin LoadOnIsolatedAppDomain(Type addinType, AddinInfo info)
        {
            AddinProxy proxy = base.LoadOnIsolatedAppDomain(addinType, info) as AddinProxy;
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
