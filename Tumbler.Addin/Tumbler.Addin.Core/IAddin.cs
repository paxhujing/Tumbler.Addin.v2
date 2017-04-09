using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示一个插件。
    /// </summary>
    public interface IAddin : IMessagePoint
    {
        ///// <summary>
        ///// 与该插件关联的代理。
        ///// </summary>
        //AddinProxy Proxy { get; }

        /// <summary>
        /// 当插件实例被创建时调用该方法。
        /// </summary>
        void Load();

        /// <summary>
        /// 当插件实例被卸载时调用该方法。
        /// </summary>
        void Unload();
    }
}
