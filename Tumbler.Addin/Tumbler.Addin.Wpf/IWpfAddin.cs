using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// WPF插件。
    /// </summary>
    public interface IWpfAddin : IAddin
    {
        /// <summary>
        /// 当收到内部消息时执行。
        /// </summary>
        /// <param name="message">消息。</param>
        void OnInternalReceive(InternalMessage message);
    }
}
