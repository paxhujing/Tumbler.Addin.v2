using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 消息辅助类。
    /// </summary>
    public static class MessageHelper
    {
        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="addin">发送该消息的插件。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        public static void Send(this IAddin addin, String destination, ContentType contentType, params Byte[] content)
        {
            if (String.IsNullOrWhiteSpace(addin.Id)) throw new ArgumentNullException("IAddin.Id");
            if (addin.Proxy == null) throw new ArgumentNullException("IAddin.Proxy");
            addin.Proxy.Send(new Message(destination, addin.Id, contentType, content));
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="addin">发送该消息的插件。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        public static void Send(this IAddin addin, String destination, ContentType contentType, String content)
        {
            if (String.IsNullOrWhiteSpace(content))
            {
                addin.Send(destination, ContentType.None);
            }
            else
            {
                Byte[] data = Encoding.UTF8.GetBytes(content);
                addin.Send(destination, contentType, data);
            }
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="addin">宿主。</param>
        /// <param name="destination">目的地。</param>
        public static void Send(this IAddin addin, String destination)
        {
            addin.Send(destination, ContentType.None, String.Empty);
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="addin">宿主。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        public static void Send(this IAddinHost host, String destination, ContentType contentType, params Byte[] content)
        {
            if (host.MessageService == null) throw new ArgumentNullException("IAddinHost.MessageService");
            host.Send(new Message(destination, MessageService.AddinHostId, contentType, content));
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="addin">宿主。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        public static void Send(this IAddinHost host, String destination, ContentType contentType, String content)
        {
            if (host.MessageService == null) throw new ArgumentNullException("IAddinHost.MessageService");
            if (String.IsNullOrWhiteSpace(content))
            {
                host.Send(new Message(destination, MessageService.AddinHostId, ContentType.None));
            }
            else
            {
                host.Send(new Message(destination, MessageService.AddinHostId, contentType, content));
            }
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="addin">宿主。</param>
        /// <param name="destination">目的地。</param>
        public static void Send(this IAddinHost host, String destination)
        {
            host.Send(destination, ContentType.None, String.Empty);
        }
    }
}
