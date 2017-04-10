using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示一个转发消息。
    /// </summary>
    [Serializable]
    public class ForwardedMessage : Message
    {
        #region Constructors

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.ForwardedMessage 。
        /// </summary>
        /// <param name="messageCode">消息码。</param>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        internal ForwardedMessage(Int32 messageCode, String destination, String source, ContentType contentType, Object content)
            : base(messageCode, destination, source, contentType, content)
        {
            Stations = new Collection<String>();
        }

        /// <summary>
        /// 初始化结构 Tumbler.Addin.Core.ForwardedMessage 。
        /// </summary>
        /// <param name="info">SerializationInfo 类型实例。</param>
        /// <param name="context">StreamingContext 类型实例。</param>
        internal ForwardedMessage(SerializationInfo info, StreamingContext context)
           : base(info, context)
        {
            String temp = info.GetString("stations");
            String[] stations = temp.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            Stations = new Collection<String>(stations);
        }

        #endregion


        #region Properties

        /// <summary>
        /// 表示消息被转发的发送者列表。
        /// </summary>
        public Collection<String> Stations { get; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 序列化使用。
        /// </summary>
        /// <param name="info">SerializationInfo 类型实例。</param>
        /// <param name="context">StreamingContext 类型实例。</param>
        [SecurityPermission(SecurityAction.Demand, RemotingConfiguration = true, SerializationFormatter = true)]
        public sealed override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            String temp = String.Join(";", Stations);
            info.AddValue("stations", temp);
        }

        #endregion

        #endregion
    }
}
