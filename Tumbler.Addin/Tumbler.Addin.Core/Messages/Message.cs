using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Messages
{
    /// <summary>
    /// 表示一个消息。
    /// </summary>
    public class Message
    {
        #region Constructors

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Message 。
        /// </summary>
        /// <param name="header">消息头。</param>
        /// <param name="content">消息内容。</param>
        public Message(MessageHeader header, Object content)
        {
            Header = header;
            Content = content;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息头。
        /// </summary>
        public MessageHeader Header { get; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public Object Content { get; internal set; }

        #endregion
    }
}
