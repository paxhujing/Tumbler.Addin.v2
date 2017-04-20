using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 描述激活器启动插件需要的条件。
    /// </summary>
    public interface ILaunchCondition
    {
        /// <summary>
        /// 启动所需资源名称。
        /// </summary>
        String ResourceName { get; }

        /// <summary>
        /// 是否要独占该资源。
        /// </summary>
        Boolean IsExclusive { get; }
    }
}
