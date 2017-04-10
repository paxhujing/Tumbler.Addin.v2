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

        private readonly XmlSchemaSet _addinXmlSchema;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinLoader 实例。
        /// </summary>
        public AddinLoader()
        {
            Stream stream = GetAddinSchemaStream();
            XmlReader schemaXml = XmlReader.Create(stream);
            _addinXmlSchema = new XmlSchemaSet();
            _addinXmlSchema.Add(null, schemaXml);
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += OnReflectionOnlyAssemblyResolve;
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        ///加载插件。
        /// </summary>
        /// <param name="addinNode">插件节点。</param>
        /// <returns>插件代理。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IMessageTarget LoadAddin(XElement addinNode)
        {
            if (addinNode.Name != AddinConfigParser.AddinNodeName) return null;
            String directory;
            XDocument doc = GetAddinConfigImpl(addinNode, out directory);
            if (doc == null) return null;
            return LoadImpl(doc, directory);
        }

        /// <summary>
        /// 加载服务。
        /// </summary>
        /// <param name="serviceNode">服务节点。</param>
        /// <returns>服务代理。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IMessageTarget LoadService(XElement serviceNode)
        {
            if (serviceNode.Name != AddinConfigParser.ServiceNodeName) return null;
            String directory;
            XDocument doc = GetAddinConfigImpl(serviceNode, out directory);
            if (doc == null) return null;
            return LoadImpl(doc, directory);
        }

        /// <summary>
        /// 卸载代理
        /// </summary>
        /// <param name="addinProxy"></param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unload(AddinProxy addinProxy)
        {
            if (addinProxy == null) throw new ArgumentNullException("addinProxy");
#if DEBUG
            Console.WriteLine($"Destroy addin {addinProxy.Id} and owner {addinProxy.Owner.FriendlyName}");
#endif
            addinProxy.Unload();
            AppDomain.Unload(addinProxy.Owner);
        }

        #endregion

        #region Protected

        /// <summary>
        /// 获取插件架构。
        /// </summary>
        /// <returns>插件架构。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual Stream GetAddinSchemaStream()
        {
            return Assembly.GetExecutingAssembly().GetManifestResourceStream("Tumbler.Addin.Core.AddinConfigSchema.xsd");
        }

        /// <summary>
        /// 将插件加载到独立的应用程序域中。
        /// </summary>
        /// <param name="addinType">实现了 IAddin 接口的类型。</param>
        /// <param name="doc">配置。</param>
        /// <returns>派生自 AddinProxy 类的实例。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual IMessageTarget LoadOnIsolatedAppDomain(Type addinType, XDocument doc)
        {
            String id = doc.Root.Attribute("id").Value;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = id;
            setup.ApplicationBase = Path.GetDirectoryName(addinType.Assembly.Location);
            AppDomain domain = AppDomain.CreateDomain($"AddinDomain#{id}", null, setup);
            try
            {
                AddinProxy proxy = (AddinProxy)domain.CreateInstanceAndUnwrap(addinType.Assembly.FullName, addinType.FullName + "Proxy");
                if (proxy.Id != id) throw new InvalidDataException("The id in config not match");
                proxy.Owner = domain;
                proxy.Load();
                domain.SetData("proxy", proxy);
                domain.SetData("id", id);
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
        /// <param name="doc">配置。</param>
        /// <returns>实现了 IAddin 接口的类型。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual IMessageTarget LoadOnDefaultAppDomain(Type addinType, XDocument doc)
        {
            try
            {
                String id = doc.Root.Attribute("id").Value;
                Assembly assembly = AppDomain.CurrentDomain.Load(addinType.Assembly.FullName);
                IAddin addin = assembly.CreateInstance(addinType.FullName) as IAddin;
                if (addin != null)
                {
                    if (addin.Id != id) return null;
                    addin.Load();
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
        /// <param name="type">类型名称。</param>
        /// <param name="directory">目录。</param>
        /// <returns>反射上下文中的类型。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected Type ParseType(String type,String directory)
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
        private XDocument GetAddinConfigImpl(XElement node,out String directory)
        {
            directory = null;
            String location = node.Attribute("location").Value;
            try
            {
                XDocument config = XDocument.Load(location);
                config.Validate(_addinXmlSchema, ValidationEventHandler);
                directory = Path.GetDirectoryName(location);
                return config;
            }
            catch (ArgumentException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (XmlSchemaValidationException)
            {
                return null;
            }
        }

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private IMessageTarget LoadImpl(XDocument doc, String directory)
        {
            String type = doc.Root.Attribute("type").Value;
            Type addinType = ParseType(type, directory);
            if (addinType == null) return null;
            Boolean needProxy = addinType.GetCustomAttributesData().SingleOrDefault(x =>
                x.AttributeType.AssemblyQualifiedName == typeof(AddinProxyAttribute).AssemblyQualifiedName) != null;
            if (needProxy)
            {
                return LoadOnIsolatedAppDomain(addinType, doc);
            }
            else
            {
                return LoadOnDefaultAppDomain(addinType, doc);
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

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void ValidationEventHandler(Object sender, ValidationEventArgs e)
        {
            throw new XmlSchemaValidationException(e.Message);
        }

        #endregion

        #endregion
    }
}
