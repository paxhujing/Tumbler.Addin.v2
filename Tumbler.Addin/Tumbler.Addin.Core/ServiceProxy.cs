using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 服务代理。用于服务与宿主之间的通信。
    /// </summary>
    public class ServiceProxy : AddinProxy, IService
    {
        #region Fields

        /// <summary>
        /// 与该代理关联的插件。
        /// </summary>
        private readonly IService _target;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.ServiceProxy 实例。
        /// </summary>
        /// <param name="target">服务。</param>
        public ServiceProxy(IService target)
            : base(target)
        {
            _target = target;
        }

        #endregion

        #region Methods

        /// <summary>
        /// 是否正在运行中。
        /// </summary>
        public Boolean IsRuning => _target.IsRuning;

        /// <summary>
        /// 启动服务。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Start()
        {
            return _target.Start();
        }

        /// <summary>
        /// 停止。
        /// </summary>
        public void Stop()
        {
            _target.Stop();
        }

        #endregion
    }
}
