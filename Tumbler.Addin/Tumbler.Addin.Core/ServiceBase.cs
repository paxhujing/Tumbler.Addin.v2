using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 服务基类。
    /// </summary>
    public abstract class ServiceBase : IService
    {
        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.ServiceBase 实例。
        /// </summary>
        /// <param name="id">服务Id号。</param>
        public ServiceBase(String id)
        {
            Id = id;
            MessageDispatcher = new MessageDispatcher(this);
        }

        /// <summary>
        /// 服务Id号。
        /// </summary>
        public String Id { get; }

        /// <summary>
        /// 消息调度器。
        /// </summary>
        public MessageDispatcher MessageDispatcher { get; }

        /// <summary>
        /// 是否正在运行中。
        /// </summary>
        public Boolean IsRuning { get; protected set; }

        /// <summary>
        /// 销毁服务。
        /// </summary>
        public virtual void Destroy()
        {
            Stop();
        }

        /// <summary>
        /// 初始化服务。
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// 收到消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public virtual void OnReceive(Message message)
        {
        }

        /// <summary>
        /// 启动服务。
        /// </summary>
        /// <returns>成功返回true；否则返回false。</returns>
        public abstract bool Start();

        /// <summary>
        /// 停止。
        /// </summary>
        public abstract void Stop();

    }
}
