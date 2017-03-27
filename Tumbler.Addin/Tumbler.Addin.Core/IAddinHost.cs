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
    public interface IAddinHost : IMessageSource, IMessageTarget
    {
        #region Properties

        /// <summary>
        /// 消息中心。
        /// </summary>
        MessageService MessageService { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 初始化。
        /// </summary>
        void Initialize();

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="configFile">插件配置文件</param>
        /// <returns>插件代理。</returns>
        IEnumerable<AddinProxy> Load(String configFile);

        Boolean Install();

        Boolean Uninstall();

        #endregion
    }
}
