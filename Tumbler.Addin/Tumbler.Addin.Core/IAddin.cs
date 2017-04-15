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
        /// <summary>
        /// 当插件实例被创建时调用该方法。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 当插件实例被卸载时调用该方法。
        /// </summary>
        void Destroy();
    }
}
