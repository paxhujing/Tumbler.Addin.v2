using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// WPF消息辅助类。
    /// </summary>
    public static class InternalMessageExtension
    {
        /// <summary>
        /// 发送内部消息。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="messageCode">消息码。</param>
        /// <param name="contentType">内容类型。</param>
        /// <param name="content">内容。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static void SendInternalMessage(this IAddin sender, Int32 messageCode, ContentType contentType, Object content)
        {
            InternalMessage message = new InternalMessage(messageCode, contentType, content);
            SendInternalMessage(sender, message);
        }

        /// <summary>
        /// 发送内部消息。
        /// </summary>
        /// <param name="sender">发送者。</param>
        /// <param name="message">消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static void SendInternalMessage(this IAddin sender, InternalMessage message)
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain()) return;
            WpfAddinProxy proxy = AppDomain.CurrentDomain.GetData("proxy") as WpfAddinProxy;
            proxy?.OnSendInternalMessage(message);
        }

        /// <summary>
        /// 将外部消息转换为内部消息。
        /// </summary>
        /// <param name="message">外部消息。</param>
        /// <returns>内部消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static InternalMessage ConvertToInternalMessage(this Message message)
        {
            return new InternalMessage(message.MessageCode, message.ContentType, message.Content);
        }
    }
}
