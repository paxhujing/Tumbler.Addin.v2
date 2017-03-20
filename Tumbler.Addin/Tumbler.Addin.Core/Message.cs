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
    public sealed class Message : MarshalByRefObject
    {
        #region Constructors

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.Message 。
        /// </summary>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        public Message(String destination, String source, ContentType contentType, params Byte[] content)
        {
            if (String.IsNullOrWhiteSpace(destination)) throw new ArgumentNullException("destination");
            if (String.IsNullOrWhiteSpace(source)) throw new ArgumentNullException("source");
            Destination = destination;
            Source = source;
            ContentType = contentType;
            Content = content;
        }

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.Message 。
        /// </summary>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        public Message(String destination, String source, ContentType contentType, String content)
            :this(destination,source,contentType,Encoding.UTF8.GetBytes(content))
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息目标。
        /// </summary>
        public String Destination { get; }

        /// <summary>
        /// 消息源。
        /// </summary>
        public String Source { get; }

        /// <summary>
        /// 消息内容类型。
        /// </summary>
        public ContentType ContentType { get; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public Byte[] Content { get; }

        #endregion
    }
}
