using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 描述该实例可以接收来自消息服务的消息。
    /// 消息会存入对象的消息队列中，并在合适的时机处理这些消息。
    /// </summary>
    public interface IMessageTarget
    {
        /// <summary>
        /// 消息队列。
        /// </summary>
        ConcurrentQueue<Message> MessageQueue { get; }

        /// <summary>
        /// 当处理接收到的消息时执行。
        /// </summary>
        /// <param name="message">消息。</param>
        void OnReceive(Message message);
    }
}
