using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 默认全局插件配置文件路径。
        /// </summary>
        public static readonly String DefaultDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "addins");

        private readonly Dictionary<AddinProxy, AppDomain> _addinRecords = new Dictionary<AddinProxy, AppDomain>();

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
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        ///加载插件。
        /// </summary>
        /// <param name="addinNode">插件节点。</param>
        /// <param name="appDomain">插件所在应用程序域。</param>
        /// <returns>插件代理。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public AddinProxy LoadAddin(XElement addinNode,out AppDomain appDomain)
        {
            appDomain = null;
            if (addinNode.Name != AddinConfigParser.AddinNodeName) return null;
            String directory;
            XDocument doc = GetAddinConfigImpl(addinNode, out directory);
            if (doc == null) return null;
            String type = doc.Root.Attribute("type").Value;
            String id = doc.Root.Attribute("id").Value;
            return CreateAppDomain(id, type, directory, out appDomain);
        }

        /// <summary>
        /// 加载服务。
        /// </summary>
        /// <param name="serviceNode">服务节点。</param>
        /// <param name="appDomain">服务所在应用程序域。</param>
        /// <returns>服务代理。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public AddinProxy LoadService(XElement serviceNode, out AppDomain appDomain)
        {
            appDomain = null;
            if (serviceNode.Name != AddinConfigParser.ServiceNodeName) return null;
            String directory;
            XDocument doc = GetAddinConfigImpl(serviceNode, out directory);
            if (doc == null) return null;
            String type = doc.Root.Attribute("type").Value;
            String id = doc.Root.Attribute("id").Value;
            return CreateAppDomain(id, type, directory, out appDomain);
        }

        /// <summary>
        /// 卸载代理
        /// </summary>
        /// <param name="addinProxy"></param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unload(AddinProxy addinProxy)
        {
            if (_addinRecords.ContainsKey(addinProxy))
            {
                AppDomain domain = _addinRecords[addinProxy];
                AppDomain.Unload(domain);
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
                if (!Path.IsPathRooted(location))
                {
                    location = Path.Combine(DefaultDirectory, location);
                }
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
        private AddinProxy CreateAppDomain(String id, String type, String directory,out AppDomain appDomain)
        {
            appDomain = null;
            Int32 index = type.IndexOf(',');
            String typeName = type.Substring(0, index);
            String assemblyName = type.Substring(index + 1);
            if (String.IsNullOrWhiteSpace(typeName) || String.IsNullOrWhiteSpace(assemblyName)) return null;
            AppDomainSetup setup = new AppDomainSetup();
            setup.ApplicationBase = directory;
            AppDomain domain = AppDomain.CreateDomain($"AddinDomain#{id}", null, setup);
            try
            {
                AddinProxy proxy = (AddinProxy)domain.CreateInstanceAndUnwrap(assemblyName, typeName + "Proxy");
                if (proxy.Id != id) throw new InvalidDataException("The id in config not match");
                appDomain = domain;
                _addinRecords.Add(proxy, domain);
                proxy.Load();
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
