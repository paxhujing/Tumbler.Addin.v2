using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示一个消息。
    /// </summary>
    [Serializable]
    public sealed class Message : ISerializable
    {
        #region Constructors

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.Message 。
        /// </summary>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="isResponse">是否是响应消息。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        internal Message(String destination, String source,ContentType contentType, params Byte[] content)
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
        /// <param name="isResponse">是否是响应消息。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        internal Message(String destination, String source, Boolean isResponse, ContentType contentType, String content)
            : this(destination, source, contentType, Encoding.UTF8.GetBytes(content))
        {
        }

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.Message 。
        /// </summary>
        /// <param name="info">SerializationInfo 类型实例。</param>
        /// <param name="context">StreamingContext 类型实例。</param>
        internal Message(SerializationInfo info, StreamingContext context)
        {
            Id = info.GetInt32("Id");
            IsResponse = info.GetBoolean("IsResponse");
            Exception = info.GetValue("Exception", typeof(Exception)) as Exception;
            Destination = info.GetString("Destination");
            Source = info.GetString("Source");
            ContentType = (ContentType)info.GetByte("ContentType");
            Content = info.GetValue("Content", typeof(Byte[])) as Byte[];
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息的唯一标识。
        /// </summary>
        public Int32 Id { get; internal set; }

        /// <summary>
        /// 是否是响应消息。
        /// </summary>
        public Boolean IsResponse { get; internal set; }

        /// <summary>
        /// 是否正确响应消息。
        /// </summary>
        public Boolean IsFailed => Exception != null;

        /// <summary>
        /// 响应失败的异常信息。
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// 消息目标。
        /// </summary>
        public String Destination { get; internal set; }

        /// <summary>
        /// 消息源。
        /// </summary>
        public String Source { get; internal set; }

        /// <summary>
        /// 消息内容类型。
        /// </summary>
        public ContentType ContentType { get; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public Byte[] Content { get; }

        /// <summary>
        /// 序列化使用。
        /// </summary>
        /// <param name="info">SerializationInfo 类型实例。</param>
        /// <param name="context">StreamingContext 类型实例。</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Id", Id);
            info.AddValue("IsResponse", IsResponse);
            info.AddValue("Exception", Exception);
            info.AddValue("Destination", Destination);
            info.AddValue("Source", Source);
            info.AddValue("ContentType", (Byte)ContentType);
            info.AddValue("Content", Content);
        }

        #endregion
    }
}
