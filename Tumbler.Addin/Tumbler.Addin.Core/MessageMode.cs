using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示消息的类型。
    /// </summary>
    public enum MessageMode
    {
        /// <summary>
        /// 用于数据传输的消息。
        /// </summary>
        Data,
        /// <summary>
        /// 用于设置回调的消息。当与该回调相关消息到达时，将执行回调方法。
        /// </summary>
        SetCallback,
        /// <summary>
        /// 用于通知服务执行相关回调（如果有人关心该消息的话）。
        /// </summary>
        NotifyCallback,
    }
}
