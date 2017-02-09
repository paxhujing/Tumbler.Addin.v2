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
    public interface IAddin
    {
        /// <summary>
        /// 插件的ID号。
        /// </summary>
        String ID { get; }

        /// <summary>
        /// 与该插件关联的代表。
        /// </summary>
        Delegator Delegator { get; }

        /// <summary>
        /// 当接收到的消息时执行。
        /// </summary>
        /// <param name="message">消息。</param>
        void OnReceive(Message message);

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        Boolean Send(Message message);
    }
}
