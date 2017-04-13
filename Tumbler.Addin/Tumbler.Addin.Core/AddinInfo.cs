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
    public class AddinInfo
    {
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
    }
}
