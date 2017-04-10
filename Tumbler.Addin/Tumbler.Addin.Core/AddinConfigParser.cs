using System;
using System.Collections.Generic;
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
    internal class AddinConfigParser
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
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取插件组中的插件节点。
        /// </summary>
        /// <param name="groupName">组名称。</param>
        /// <returns>插件节点。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<XElement> GetAddinNodes(String groupName)
        {
            XElement addinGroupNode = _addinGroups?.Elements().FirstOrDefault(x =>x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            return addinGroupNode.Elements(AddinNodeName);
        }

        /// <summary>
        /// 获取插件组中子组的插件节点。
        /// </summary>
        /// <param name="groupName">组名称。</param>
        /// <param name="subName">子组名称。</param>
        /// <returns>插件节点。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<XElement> GetSubAddinNodes(String groupName, String subName)
        {
            XElement addinGroupNode = _addinGroups?.Elements().FirstOrDefault(x => x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            XElement subNode = addinGroupNode.Elements(AddinGroupSubNodeName).FirstOrDefault(x => x.Attribute("name").Value == subName);
            if (subNode == null) return null;
            return subNode.Elements(AddinNodeName);
        }

        /// <summary>
        /// 获取服务节点。
        /// </summary>
        /// <returns>服务节点。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<XElement> GetServiceNodes()
        {
            return _services?.Elements(ServiceNodeName);
        }

        #endregion

        #region Private

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void ValidationEventHandler(Object sender, ValidationEventArgs e)
        {
            throw new XmlSchemaValidationException(e.Message);
        }

        #endregion

        #endregion
    }
}
