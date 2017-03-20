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
    public interface IAddinHost
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

        /// <summary>
        /// 收到来自其它插件的消息。
        /// </summary>
        /// <param name="message">消息。</param>
        void OnReceive(Message message);

        /// <summary>
        /// 将消息发给其它插件。
        /// </summary>
        /// <param name="message"></param>
        void Send(Message message);

        Boolean Install();

        Boolean Uninstall();

        #endregion
    }
}
