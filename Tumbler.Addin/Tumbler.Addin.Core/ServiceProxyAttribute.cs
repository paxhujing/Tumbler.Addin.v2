using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示服务所需的代理。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class ServiceProxyAttribute : Attribute
    {
        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.ServiceProxyAttribute 实例。
        /// </summary>
        /// <param name="type">继承自 ServiceProxy 的类型。</param>
        public ServiceProxyAttribute(Type type)
        {
            if (type == null) throw new ArgumentNullException("type");
            if (type.BaseType != typeof(ServiceProxy))
            {
                throw new ArgumentException("Must inherit from ServiceProxy");
            }
            Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// 继承自 ServiceProxy 的类型。
        /// </summary>
        public Type Type { get; set; }

        #endregion
    }
}
