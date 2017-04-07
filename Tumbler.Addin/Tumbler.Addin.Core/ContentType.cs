using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 消息中内容的类型。
    /// </summary>
    public enum ContentType : Byte
    {
        /// <summary>
        /// 无消息内容。
        /// </summary>
        None,
        /// <summary>
        /// JSON格式的字符串。
        /// </summary>
        JSON,
        /// <summary>
        /// XML文档。
        /// </summary>
        XML,
        /// <summary>
        /// 纯文本。
        /// </summary>
        Text,
        /// <summary>
        /// 字节数组。
        /// </summary>
        ByteArray,
        /// <summary>
        /// 对象。
        /// </summary>
        Object,
        /// <summary>
        /// 异常信息。
        /// </summary>
        Exception,
    }
}
