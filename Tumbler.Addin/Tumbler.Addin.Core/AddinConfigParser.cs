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
    public class AddinConfigParser
    {
        #region Fields

        private XElement _addinGroups;

        private XElement _services;

        private const String ServicesNodeName = "services";

        private const String addinGroupsNodeName = "addinGroups";

        private const String AddinNodeName = "addin";

        private const String ServiceNodeName = "service";

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
            
            XDocument doc = XDocument.Load(configFile);
            doc.Validate(schemaSet, ValidationEventHandler);
            _addinGroups = doc.Root.Element(addinGroupsNodeName);
            _services = doc.Root.Element(ServicesNodeName);

            stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Tumbler.Addin.Core.AddinConfigSchema.xsd");
            schemaXml = XmlReader.Create(stream);
            _addinXmlSchema = new XmlSchemaSet();
            _addinXmlSchema.Add(null, schemaXml);
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取插件组中的插件节点。
        /// </summary>
        /// <param name="groupName">组名称。</param>
        /// <returns>插件节点。</returns>
        public IEnumerable<XElement> GetAddins(String groupName)
        {
            XElement addinGroupNode = _addinGroups.Elements().FirstOrDefault(x =>x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            return addinGroupNode.Elements(AddinNodeName);
        }

        /// <summary>
        /// 获取插件组中的子组节点。
        /// </summary>
        /// <param name="groupName">子组名称。</param>
        /// <returns>子组节点。</returns>
        public IEnumerable<XElement> GetSubs(String groupName)
        {
            XElement addinGroupNode = _addinGroups.Elements().FirstOrDefault(x => x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            return addinGroupNode.Elements(AddinGroupSubNodeName);
        }

        /// <summary>
        /// 获取插件组中子组的插件节点。
        /// </summary>
        /// <param name="groupName">组名称。</param>
        /// <param name="subName">子组名称。</param>
        /// <returns>插件节点。</returns>
        public IEnumerable<XElement> GetSubAddins(String groupName, String subName)
        {
            XElement addinGroupNode = _addinGroups.Elements().FirstOrDefault(x => x.Attribute("name").Value == groupName);
            if (addinGroupNode == null) return null;
            XElement subNode = addinGroupNode.Elements(AddinGroupSubNodeName).FirstOrDefault(x => x.Attribute("name").Value == subName);
            if (subNode == null) return null;
            return subNode.Elements(AddinNodeName);
        }

        /// <summary>
        /// 获取服务节点。
        /// </summary>
        /// <returns>服务节点。</returns>
        public IEnumerable<XElement> GetServices()
        {
            return _services.Elements(ServiceNodeName);
        }

        #endregion

        #region Private

        private void ValidationEventHandler(Object sender, ValidationEventArgs e)
        {
            throw new XmlSchemaValidationException(e.Message);
        }

        #endregion

        #endregion
    }
}
