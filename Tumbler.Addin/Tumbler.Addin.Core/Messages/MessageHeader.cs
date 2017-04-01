using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Messages
{
    /// <summary>
    /// 消息头。
    /// </summary>
    public abstract class MessageHeader
    {
        #region Constructors

        /// <summary>
        /// 初始化类型实例。
        /// </summary>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容类型。</param>
        protected MessageHeader(String destination, String source, ContentType contentType)
        {
            if (String.IsNullOrWhiteSpace(destination)) throw new ArgumentNullException("destination");
            if (String.IsNullOrWhiteSpace(source)) throw new ArgumentNullException("source");
            Destination = destination;
            Source = source;
            ContentType = contentType;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 版本号。
        /// </summary>
        public abstract String Version { get; }

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

        //头部长度。
        public abstract Int32 HeaderLength { get; }

        #endregion
    }
}
