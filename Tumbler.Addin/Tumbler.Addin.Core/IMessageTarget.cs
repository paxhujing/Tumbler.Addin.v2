using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示可以接收来自消息服务的消息。
    /// </summary>
    public interface IMessageTarget : IObjectIdentity
    {
        /// <summary>
        /// 消息调度器。
        /// </summary>
        MessageDispatcher MessageDispatcher { get; }

        /// <summary>
        /// 当收到的消息时执行。
        /// </summary>
        /// <param name="message">消息。</param>
        void OnReceive(Message message);
    }
}
