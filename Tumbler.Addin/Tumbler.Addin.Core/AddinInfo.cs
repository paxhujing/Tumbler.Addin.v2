using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public static readonly AddinInfo Invalid = new AddinInfo();

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinInfo 实例。
        /// </summary>
        internal AddinInfo()
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 配置中的location值。
        /// </summary>
        public String Location { get; internal set; }

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
        public String Id { get; internal set; }

        /// <summary>
        /// 入口类型。
        /// </summary>
        public String Type { get; internal set; }

        /// <summary>
        /// 版本号。
        /// </summary>
        public String Version { get; internal set; }

        /// <summary>
        /// 更新地址。
        /// </summary>
        public String UpdateUrl { get; internal set; }

        /// <summary>
        /// 名称。
        /// </summary>
        public String Name { get; internal set; }

        /// <summary>
        /// 作者。
        /// </summary>
        public String Author { get; internal set; }

        /// <summary>
        /// 版权。
        /// </summary>
        public String Copyright { get; internal set; }

        /// <summary>
        /// 插件文档Url。
        /// </summary>
        public String Url { get; internal set; }

        /// <summary>
        /// 描述。
        /// </summary>
        public String Description { get; internal set; }

        #endregion

        #region Methods

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
    }
}
