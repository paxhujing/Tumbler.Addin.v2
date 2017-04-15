using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// WPF插件基类。
    /// </summary>
    public abstract class WpfAddinBase : AddinBase, IWpfAddin
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.WpfAddinBase 实例。
        /// </summary>
        /// <param name="id">唯一标识。</param>
        protected WpfAddinBase(String id) 
            : base(id)
        {
        }

        #endregion

        #region Methods

        /// <summary>
        /// 当收到内部消息时执行。
        /// </summary>
        /// <param name="message">消息。</param>
        public virtual void OnInternalReceive(InternalMessage message)
        {
        }

        #endregion
    }
}
