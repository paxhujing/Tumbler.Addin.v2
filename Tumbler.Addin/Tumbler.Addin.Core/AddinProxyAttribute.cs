using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示插件所需的代理。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class AddinProxyAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinProxyAttribute 实例。
        /// </summary>
        /// <param name="type">继承自 AddinProxy 的类型。</param>
        public AddinProxyAttribute(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            do
            {
                if (type.BaseType == typeof(AddinProxy))
                {
                    return;
                }
            } while (type.BaseType != typeof(Object));
            throw new ArgumentException("Must inherit from AddinProxy");
        }

        #endregion

        #region Properties

        /// <summary>
        /// 继承自 AddinProxy 的类型。
        /// </summary>
        public Type Type { get; set; }

        #endregion
    }
}
