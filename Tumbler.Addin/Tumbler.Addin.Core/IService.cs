using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示一个服务。
    /// </summary>
    public interface IService : IAddin
    {
        /// <summary>
        /// 启动服务。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        Boolean Start();

        /// <summary>
        /// 停止。
        /// </summary>
        void Stop();
    }
}
