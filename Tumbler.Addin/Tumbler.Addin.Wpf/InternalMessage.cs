using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 内部消息。
    /// </summary>
    public class InternalMessage : MarshalByRefObject
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.InternalMessage 实例。
        /// </summary>
        /// <param name="messageCode">消息码。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        internal InternalMessage(Int32 messageCode, ContentType contentType, Object content)
        {
            MessageCode = messageCode;
            ContentType = contentType;
            Content = content;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 消息的唯一标识。
        /// </summary>
        public Int32 Id { get; } = Guid.NewGuid().GetHashCode();

        /// <summary>
        /// 消息码。用于表明消息的类型。
        /// </summary>
        public Int32 MessageCode { get; }

        /// <summary>
        /// 是否正确响应消息。
        /// </summary>
        public Boolean IsFailed => ContentType == ContentType.Exception;

        /// <summary>
        /// 消息内容类型。
        /// </summary>
        public ContentType ContentType { get; }

        /// <summary>
        /// 消息内容。
        /// </summary>
        public Object Content { get; }

        #endregion
    }
}
