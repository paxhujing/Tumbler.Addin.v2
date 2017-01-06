using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示通信过程中的一个消息包裹。
    /// </summary>
    public sealed class MessagePackage : MarshalByRefObject
    {
        #region Fields

        private readonly String _content;

        #endregion

        #region Constructors

        internal MessagePackage(Guid source, Guid target, String content, Type contentType, MessageMode mode)
        {
            Source = source;
            Target = target;
            _content = content;
            ContentType = contentType;
            Mode = mode;
        }

        #endregion

        #region Properties

        public Guid Source { get; }

        public Guid Target { get; }

        public Type ContentType { get; }

        public Object Content => JsonConvert.DeserializeObject(_content, ContentType);

        public MessageMode Mode { get; }

        #endregion
    }
}
