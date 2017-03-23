using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示该类型实例具有唯一的标识。
    /// </summary>
    public interface IObjectIdentity
    {
        /// <summary>
        /// 唯一标识。
        /// </summary>
        String Id { get; }
    }
}
