using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件基类。
    /// </summary>
    public abstract class AddinBase : IAddin
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinBase 实例。
        /// </summary>
        /// <param name="id">唯一标识。</param>
        protected AddinBase(String id)
        {
            Id = id;
            MessageDispatcher = new MessageDispatcher(this);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 唯一标识。
        /// </summary>
        public String Id { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 消息调度器。
        /// </summary>
        public MessageDispatcher MessageDispatcher { get; }

        /// <summary>
        /// 当插件实例被创建时调用该方法。
        /// </summary>
        public virtual void Initialize()
        {
        }

        /// <summary>
        /// 当收到的消息时执行。
        /// </summary>
        /// <param name="message">消息。</param>
        public virtual void OnReceive(Message message)
        {
        }

        /// <summary>
        /// 当插件实例被卸载时调用该方法。
        /// </summary>
        public virtual void Destroy()
        {
        }

        #endregion
    }
}
