﻿using System;
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
        /// <summary>
        /// 插件管理器。
        /// </summary>
        AddinManager AddinManager { get; }

        /// <summary>
        /// 初始化。
        /// </summary>
        void Initialize();

        Boolean Install();

        Boolean Uninstall();
    }
}
