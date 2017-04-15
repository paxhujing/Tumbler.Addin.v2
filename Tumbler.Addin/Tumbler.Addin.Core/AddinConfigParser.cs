using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件配置解析器。
    /// </summary>
    public class AddinConfigParser
    {
        #region Fields

        /// <summary>
        /// 插件目录。
        /// </summary>
        public static readonly String AddinDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "addins");

        private XElement _addinGroups;

        private XElement _services;

        private const String ServicesNodeName = "services";

        private const String addinGroupsNodeName = "addinGroups";

        internal const String AddinNodeName = "addin";

        internal const String ServiceNodeName = "service";

        private const String AddinGroupNodeName = "addinGroup";

        private const String AddinGroupSubNodeName = "sub";

        private readonly XmlSchemaSet _addinXmlSchema;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinConfigParser 实例。
        /// </summary>
        /// <param name="configFile">配置文件。</param>
        public AddinConfigParser(String configFile)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tumbler.Addin.Core.AddinGlobalConfigSchema.xsd");
            XmlReader schemaXml = XmlReader.Create(stream);
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, schemaXml);

            if (!Path.IsPathRooted(configFile))
            {
                configFile = Path.Combine(AddinDirectory, configFile);
            }
            XDocument doc = XDocument.Load(configFile);
            doc.Validate(schemaSet, ValidationEventHandler);
            _addinGroups = doc.Root.Element(addinGroupsNodeName);
            _services = doc.Root.Element(ServicesNodeName);

            stream = GetAddinSchemaStream();
            schemaXml = XmlReader.Create(stream);
            _addinXmlSchema = new XmlSchemaSet();
            _addinXmlSchema.Add(null, schemaXml);
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取所有插件组的名称。
        /// </summary>
        /// <returns>插件组的名称列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<String> GetAddinGroupNames()
        {
            return _addinGroups?.Elements("addinGroup")?.Select(x => x.Attribute("name").Value) ?? new String[0];
        }

        /// <summary>
        /// 获取插件组的所有子组名称。
        /// </summary>
        /// <returns>插件组的名称列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<String> GetSubNames(String groupName)
        {
            XElement addinGroupNode = _addinGroups?.Elements().FirstOrDefault(x => x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            return addinGroupNode.Elements("sub")?.Select(x => x.Attribute("name").Value) ?? new String[0];
        }

        /// <summary>
        /// 获取插件信息。
        /// </summary>
        /// <param name="location">插件配置路径。</param>
        /// <returns>插件信息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public AddinInfo GetAddinInfo(String location)
        {
            String configFile = Path.Combine(AddinConfigParser.AddinDirectory, location);
            XDocument doc = GetAddinConfigImpl(configFile);
            if (doc == null) return null;
            return GetAddinInfoImpl(doc.Root, location);
        }

        /// <summary>
        /// 获取插件组中的插件信息。
        /// </summary>
        /// <param name="groupName">组名称。</param>
        /// <returns>插件信息列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinInfo> GetAddinInfos(String groupName)
        {
            XElement addinGroupNode = _addinGroups?.Elements().FirstOrDefault(x =>x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            IEnumerable<XElement> addinConfigs = addinGroupNode.Elements(AddinNodeName);
            if (addinConfigs == null) return null;
            return GetAddinInfos(addinConfigs);
        }

        /// <summary>
        /// 获取插件组中子组的插件信息。
        /// </summary>
        /// <param name="groupName">组名称。</param>
        /// <param name="subName">子组名称。</param>
        /// <returns>插件信息列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinInfo> GetSubAddinInfos(String groupName, String subName)
        {
            XElement addinGroupNode = _addinGroups?.Elements().FirstOrDefault(x => x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            XElement subNode = addinGroupNode.Elements(AddinGroupSubNodeName).FirstOrDefault(x => x.Attribute("name").Value == subName);
            if (subNode == null) return null;
            IEnumerable<XElement> addinConfigs = subNode.Elements(AddinNodeName);
            if (addinConfigs == null) return null;
            return GetAddinInfos(addinConfigs);
        }

        /// <summary>
        /// 获取服务插件信息。
        /// </summary>
        /// <returns>插件信息列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinInfo> GetServiceInfos()
        {
            IEnumerable<XElement> addinConfigs = _services?.Elements(ServiceNodeName);
            if (addinConfigs == null) return null;
            return GetAddinInfos(addinConfigs);
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
        /// 获取插件信息。
        /// </summary>
        /// <param name="root">插件配置。</param>
        /// <param name="location">配置文件相对路径。</param>
        /// <returns>插件信息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual AddinInfo GetAddinInfoImpl(XElement root,String location)
        {
            return new AddinInfo(root, location);
        }

        #endregion

        #region Private

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private IEnumerable<AddinInfo> GetAddinInfos(IEnumerable<XElement> addinConfigs)
        {
            AddinInfo temp = null;
            Collection<AddinInfo> infos = new Collection<AddinInfo>();
            foreach (XElement addinConfig in addinConfigs)
            {
                temp = GetAddinInfo(addinConfig.Attribute("location").Value);
                if (temp == null) continue;
                infos.Add(temp);
            }
            return infos;
        }

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private XDocument GetAddinConfigImpl(String configFile)
        {
            if (!File.Exists(configFile)) return null;
            try
            {
                XDocument config = XDocument.Load(configFile);
                config.Validate(_addinXmlSchema, ValidationEventHandler);
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
        private void ValidationEventHandler(Object sender, ValidationEventArgs e)
        {
            throw new XmlSchemaValidationException(e.Message);
        }

        #endregion

        #endregion
    }
}
