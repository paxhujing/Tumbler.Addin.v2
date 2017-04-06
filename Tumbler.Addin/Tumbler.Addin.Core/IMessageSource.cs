using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示可以向其它实现了 IMessageSource 接口的实例发送消息。
    /// </summary>
    public interface IMessageSource : IObjectIdentity
    {
    }
}
