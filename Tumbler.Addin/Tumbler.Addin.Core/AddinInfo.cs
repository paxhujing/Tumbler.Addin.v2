using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件信息。
    /// </summary>
    public class AddinInfo : IEquatable<AddinInfo>
    {
        #region Fields

        /// <summary>
        /// 无效的插件。
        /// </summary>
        public static readonly AddinInfo Invalid = new AddinInfo(null, null);

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinInfo 实例。
        /// </summary>
        /// <param name="root">插件配置。</param>
        /// <param name="location">配置文件相对路径。</param>
        internal protected AddinInfo(XElement root,String location)
        {
            if (root == null) return;
            Initialize(root, location);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 配置中的location值。
        /// </summary>
        public String Location { get; private set; }

        /// <summary>
        /// 插件所在绝对路径。
        /// </summary>
        public String AbsolutePath => System.IO.Path.Combine(AddinConfigParser.AddinDirectory, Location);

        /// <summary>
        /// 插件所在目录。
        /// </summary>
        public String Directory => System.IO.Path.GetDirectoryName(AbsolutePath);

        /// <summary>
        /// Id。
        /// </summary>
        public String Id { get; private set; }

        /// <summary>
        /// 入口类型。
        /// </summary>
        public String Type { get; private set; }

        /// <summary>
        /// 版本号。
        /// </summary>
        public String Version { get; private set; }

        /// <summary>
        /// 更新地址。
        /// </summary>
        public String UpdateUrl { get; private set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// 作者。
        /// </summary>
        public String Author { get; private set; }

        /// <summary>
        /// 版权。
        /// </summary>
        public String Copyright { get; private set; }

        /// <summary>
        /// 插件文档Url。
        /// </summary>
        public String Url { get; private set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public String Description { get; private set; }

        /// <summary>
        /// 图标的Uri。
        /// </summary>
        public String Icon { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取对象的Hash值。
        /// </summary>
        /// <returns>对象的Hash值。</returns>
        public override Int32 GetHashCode()
        {
            return (AbsolutePath ?? String.Empty).GetHashCode() ^ (Id ?? String.Empty).GetHashCode();
        }

        /// <summary>
        /// 获取对象的字符串表示。
        /// </summary>
        /// <returns>对象的字符串表示。</returns>
        public override String ToString()
        {
            return AbsolutePath;
        }

        /// <summary>
        /// 判断两个对象是否相等。
        /// </summary>
        /// <param name="obj">另一个对象。</param>
        /// <returns>如果相等则返回true；否则返回false。</returns>
        public override Boolean Equals(object obj)
        {
            return Equals(obj as AddinInfo);
        }

        /// <summary>
        /// 判断两个对象是否相等。
        /// </summary>
        /// <param name="other">另一个对象。</param>
        /// <returns>如果相等则返回true；否则返回false。</returns>
        public Boolean Equals(AddinInfo other)
        {
            if (other == null) return false;
            if (Object.ReferenceEquals(other, this)) return true;
            return other.GetHashCode() == this.GetHashCode();
        }

        #endregion

        #region Protected

        /// <summary>
        /// 初始化。
        /// </summary>
        /// <param name="root">插件配置。</param>
        /// <param name="location">配置文件相对路径。</param>
        protected virtual void Initialize(XElement root,String location)
        {
            Location = location;
            Id = root.Attribute("id")?.Value;
            Type = root.Attribute("type")?.Value;
            UpdateUrl = root.Attribute("updateUrl")?.Value;
            XElement infoElement = root.Element("info");
            if (infoElement != null)
            {
                Name = infoElement.Element("name")?.Value;
                Author = infoElement.Element("author")?.Value;
                Copyright = infoElement.Element("copyright")?.Value;
                Url = infoElement.Element("url")?.Value;
                Description = infoElement.Element("description")?.Value;
                Icon = infoElement.Element("icon")?.Value;
            }
        }

        #endregion

        #endregion
    }
}
