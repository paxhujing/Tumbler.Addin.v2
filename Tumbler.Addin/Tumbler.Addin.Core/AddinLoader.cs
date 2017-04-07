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
    internal class AddinLoader
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
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tumbler.Addin.Core.AddinConfigSchema.xsd");
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
            Int32 index = type.IndexOf(',');
            String typeName = type.Substring(0, index);
            String assemblyName = type.Substring(index + 1);
            if (String.IsNullOrWhiteSpace(typeName) || String.IsNullOrWhiteSpace(assemblyName)) return null;
            try
            {
                Assembly assembly = Assembly.ReflectionOnlyLoadFrom(Path.Combine(directory, assemblyName + ".dll"));
                Type addinType = assembly.GetType(typeName);
                Boolean needProxy = addinType.GetCustomAttributesData().SingleOrDefault(x => 
                    x.AttributeType.AssemblyQualifiedName == typeof(AddinProxyAttribute).AssemblyQualifiedName) != null;
                if (needProxy)
                {
                    return LoadOnIsolatedAppDomain(addinType.FullName, assembly, doc);
                }
                else
                {
                    return LoadOnDefaultAppDomain(addinType.FullName, assembly, doc);
                }
            }
            catch (Exception ex)
            {
                return null;
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
        private IMessageTarget LoadOnDefaultAppDomain(String typeName, Assembly reflectionAssembly, XDocument doc)
        {
            try
            {
                String id = doc.Root.Attribute("id").Value;
                Assembly assembly = AppDomain.CurrentDomain.Load(reflectionAssembly.FullName);
                IAddin addin = assembly.CreateInstance(typeName) as IAddin;
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
            catch (Exception ex)
            {
                return null;
            }
        }

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private IMessageTarget LoadOnIsolatedAppDomain(String typeName, Assembly reflectionAssembly, XDocument doc)
        {
            String id = doc.Root.Attribute("id").Value;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationName = id;
            setup.ApplicationBase = Path.GetDirectoryName(reflectionAssembly.Location);
            AppDomain domain = AppDomain.CreateDomain($"AddinDomain#{id}", null, setup);
            try
            {
                AddinProxy proxy = (AddinProxy)domain.CreateInstanceAndUnwrap(reflectionAssembly.FullName, typeName + "Proxy");
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
            catch (Exception ex)
            {
                AppDomain.Unload(domain);
                return null;
            }
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
