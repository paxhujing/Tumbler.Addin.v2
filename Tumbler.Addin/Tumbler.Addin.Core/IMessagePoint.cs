using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示该对象可以收发消息，但不是宿主、插件和服务。
    /// </summary>
    public interface IMessagePoint : IMessageSource, IMessageTarget
    {
    }
}
