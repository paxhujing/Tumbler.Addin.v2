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
    }
}
