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
    public interface IAddin : IMessageTarget, IMessageSource
    {
        /// <summary>
        /// 与该插件关联的代理。
        /// </summary>
        AddinProxy Proxy { get; }
    }
}
