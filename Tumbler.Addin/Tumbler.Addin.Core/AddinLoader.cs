using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件加载器。
    /// </summary>
    public class AddinLoader
    {
        #region Fields

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinLoader 实例。
        /// </summary>
        public AddinLoader()
        {
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        ///加载插件。
        /// </summary>
        /// <param name="addinInfo">插件信息。</param>
        /// <returns>插件代理。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IAddin LoadAddin(AddinInfo addinInfo)
        {
            if (addinInfo == null || addinInfo == AddinInfo.Invalid) return null;
            return LoadImpl(addinInfo);
        }

        /// <summary>
        /// 卸载插件。
        /// </summary>
        /// <param name="addin">插件。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unload(IAddin addin)
        {
            if (addin == null) throw new ArgumentNullException("addin");
            addin.Destroy();
#if DEBUG
            Console.WriteLine($"Destroy addin {addin.Id}");
#endif
            AddinProxy proxy = addin as AddinProxy;
            if (proxy == null) return;
            AppDomain.Unload(proxy.Owner);
        }

        #endregion

        #region Protected

        /// <summary>
        /// 将插件加载到独立的应用程序域中。
        /// </summary>
        /// <param name="addinType">实现了 IAddin 接口的类型。</param>
        /// <param name="info">插件信息。</param>
        /// <returns>派生自 AddinProxy 类的实例。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual IAddin LoadOnIsolatedAppDomain(Type addinType, AddinInfo info)
        {
            String id = info.Id;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = id;
            setup.ApplicationBase = Path.GetDirectoryName(addinType.Assembly.Location);
            String selfConfigFile = addinType.Assembly.Location + ".config";
            if (File.Exists(selfConfigFile))
            {
                setup.ConfigurationFile = selfConfigFile;
            }
            else
            {
                setup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            }
            AppDomain domain = AppDomain.CreateDomain($"AddinDomain#{id}", null, setup);
            try
            {
                AddinProxy proxy = (AddinProxy)domain.CreateInstanceAndUnwrap(addinType.Assembly.FullName, addinType.FullName + "Proxy");
                if (proxy.Id != id) throw new InvalidDataException("The id in config not match");
                domain.SetData("proxy", proxy);
                domain.SetData("id", id);
                proxy.Owner = domain;
                proxy.Initialize();
#if DEBUG
                Console.WriteLine($"Created isolated addin {proxy.Id} in {domain.FriendlyName}");
#endif
                return proxy;
            }
            catch (MissingMemberException ex)
            {
                AppDomain.Unload(domain);
                return null;
            }
        }

        /// <summary>
        /// 将插件加载到默认的应用程序域中。
        /// </summary>
        /// <param name="addinType">实现了 IAddin 接口的类型。</param>
        /// <param name="info">插件信息。</param>
        /// <returns>实现了 IAddin 接口的类型。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual IAddin LoadOnDefaultAppDomain(Type addinType, AddinInfo info)
        {
            try
            {
                String id = info.Id;
                Assembly assembly = AppDomain.CurrentDomain.Load(addinType.Assembly.FullName);
                IAddin addin = (IAddin)assembly.CreateInstance(addinType.FullName);
                if (addin != null)
                {
                    if (addin.Id != id) return null;
                    addin.Initialize();
#if DEBUG
                    Console.WriteLine($"Created addin {addin.Id} in default appdomain");
#endif
                }
                return addin;
            }
            catch (MissingMemberException ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 在反射上下文中解析类型。
        /// </summary>
        /// <param name="type">插件类型。</param>
        /// <param name="directory">所在目录</param>
        /// <returns>反射上下文中的类型。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected Type ParseType(String type, String directory)
        {
            try
            {
                Int32 index = type.IndexOf(',');
                String typeName = type.Substring(0, index);
                String assemblyName = type.Substring(index + 1);
                if (String.IsNullOrWhiteSpace(typeName) || String.IsNullOrWhiteSpace(assemblyName)) return null;
                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(Path.Combine(directory, assemblyName + ".dll"));
                Type addinType = assembly.GetType(typeName);
                return addinType;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Private

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private IAddin LoadImpl(AddinInfo info)
        {
            Type addinType = ParseType(info.Type, info.Directory);
            if (addinType == null || addinType.GetInterface(typeof(IAddin).FullName) == null) return null;
            Boolean needProxy = addinType.GetCustomAttributesData().SingleOrDefault(x =>
                x.AttributeType.AssemblyQualifiedName == typeof(AddinProxyAttribute).AssemblyQualifiedName) != null;
            if (needProxy)
            {
                return LoadOnIsolatedAppDomain(addinType, info);
            }
            else
            {
                return LoadOnDefaultAppDomain(addinType, info);
            }
        }

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private Assembly OnReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            Assembly existed = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(x => x.FullName == args.Name);
            if (existed != null)
            {
                return Assembly.ReflectionOnlyLoadFrom(existed.Location);
            }
            String directory = Path.GetDirectoryName(args.RequestingAssembly.Location);
            AssemblyName an = new AssemblyName(args.Name);
            String file = Path.Combine(directory, an.Name + ".dll");
            return Assembly.ReflectionOnlyLoadFrom(file);
        }

        #endregion

        #endregion
    }
}
