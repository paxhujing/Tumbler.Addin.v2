using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Messages
{
    public class MessageHeaderV1: MessageHeader
    {
        #region Constructors

        /// <summary>
        /// 初始化类型实例。
        /// </summary>
        /// <param name="destination">消息目标。</param>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容类型。</param>
        protected MessageHeaderV1(String destination, String source)
            : base(destination, source, ContentType.JSON)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// 头部长度。
        /// </summary>
        public override Int32 HeaderLength { get; }

        /// <summary>
        /// 版本号。
        /// </summary>
        public override String Version { get; } = "1.0";


        #endregion
    }
}
