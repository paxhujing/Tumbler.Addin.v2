using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示一个消息。
    /// </summary>
    public struct Message
    {
        #region Constructors

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.Message 。
        /// </summary>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="content">消息内容。</param>
        public Message(String destination, String source, Object content)
        {
            Destination = destination;
            Source = source;
            Content = content;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息目标。
        /// </summary>
        public String Destination { get; internal set; }

        /// <summary>
        /// 消息源。
        /// </summary>
        public String Source { get; internal set; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public Object Content { get; internal set; }

        #endregion
    }
}
