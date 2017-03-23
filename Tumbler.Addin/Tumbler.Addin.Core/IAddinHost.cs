using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示承载插件的宿主。
    /// </summary>
    public interface IAddinHost : IMessageTarget
    {
        #region Properties

        /// <summary>
        /// 消息服务。
        /// </summary>
        MessageService MessageService { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化。
        /// </summary>
        void Initialize();

        Boolean Install();

        Boolean Uninstall();

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        void Send(Message message);

        #endregion
    }
}
