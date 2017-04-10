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
    public class Message : ISerializable, ICloneable
    {
        #region Constructors

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.Message 。
        /// </summary>
        /// <param name="messageCode">消息码。</param>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        internal Message(Int32 messageCode, String destination, String source, ContentType contentType, Object content)
        {
            if (String.IsNullOrWhiteSpace(destination)) throw new ArgumentNullException("destination");
            if (String.IsNullOrWhiteSpace(source)) throw new ArgumentNullException("source");
            MessageCode = messageCode;
            Destination = destination;
            Source = source;
            ContentType = contentType;
            Content = content;
        }

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.Message 。
        /// </summary>
        /// <param name="info">SerializationInfo 类型实例。</param>
        /// <param name="context">StreamingContext 类型实例。</param>
        internal Message(SerializationInfo info, StreamingContext context)
        {
            MessageCode = info.GetInt32("MessageCode");
            Id = info.GetInt32("Id");
            IsResponse = info.GetBoolean("IsResponse");
            Destination = info.GetString("Destination");
            Source = info.GetString("Source");
            ContentType = (ContentType)info.GetByte("ContentType");
            Content = info.GetValue("Content",typeof(Object));
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息的唯一标识。
        /// </summary>
        public Int32 Id { get; internal set; } = Guid.NewGuid().GetHashCode();

        /// <summary>
        /// 是否是响应消息。
        /// </summary>
        public Boolean IsResponse { get; internal set; }

        /// <summary>
        /// 消息码。用于表明消息的类型。
        /// </summary>
        public Int32 MessageCode { get; internal set; }

        /// <summary>
        /// 是否正确响应消息。
        /// </summary>
        public Boolean IsFailed => ContentType ==  ContentType.Exception;

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
        public Object Content { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 序列化使用。
        /// </summary>
        /// <param name="info">SerializationInfo 类型实例。</param>
        /// <param name="context">StreamingContext 类型实例。</param>
        [SecurityPermission(SecurityAction.Demand, RemotingConfiguration = true, SerializationFormatter = true)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("MessageCode", MessageCode);
            info.AddValue("Id", Id);
            info.AddValue("IsResponse", IsResponse);
            info.AddValue("Destination", Destination);
            info.AddValue("Source", Source);
            info.AddValue("ContentType", (Byte)ContentType);
            info.AddValue("Content", Content);
        }

        /// <summary>
        /// 浅克隆对象。
        /// </summary>
        /// <returns>新实例。</returns>
        [SecurityPermission(SecurityAction.Demand, RemotingConfiguration = false, SerializationFormatter = false)]
        public Object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion

        #endregion
    }
}
