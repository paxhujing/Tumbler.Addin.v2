using log4net;
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
    public interface IAddinHost : IAddin
    {
        /// <summary>
        /// 插件管理器。
        /// </summary>
        AddinManager AddinManager { get; }

        /// <summary>
        /// 日志记录器。
        /// </summary>
        ILog Logger { get; }

        Boolean Install();

        Boolean Uninstall();
    }
}
