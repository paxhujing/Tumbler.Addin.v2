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
    public static class MessageExtension
    {
        #region Message

        /// <summary>
        /// 创建消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessage(this IMessageSource source, String destination, ContentType contentType, params Byte[] content)
        {
            if (content == null || content.Length == 0)
            {
                return new Message(destination, source.Id, ContentType.None, content);
            }
            return new Message(destination, source.Id, contentType, content);
        }

        /// <summary>
        /// 创建消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessage(this IMessageSource source, String destination, ContentType contentType, String content)
        {
            Byte[] data = Encoding.UTF8.GetBytes(content);
            return CreateMessage(source, destination, contentType, data);
        }

        /// <summary>
        /// 创建发送给宿主的消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessageToHost(this IMessageSource source, ContentType contentType, params Byte[] content)
        {
            return CreateMessage(source, MessageService.AddinHostId, contentType, content);
        }

        /// <summary>
        /// 创建发送给宿主的消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessageToHost(this IMessageSource source, ContentType contentType, String content)
        {
            return CreateMessage(source, MessageService.AddinHostId, contentType, content);
        }

        /// <summary>
        /// 创建发送给所有插件（自己和宿主除外）的消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessageToAll(this IMessageSource source, ContentType contentType, params Byte[] content)
        {
            return CreateMessage(source, MessageService.AllTargetsId, contentType, content);
        }

        /// <summary>
        /// 创建发送给所有插件（自己和宿主除外）的消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessageToAll(this IMessageSource source, ContentType contentType, String content)
        {
            return CreateMessage(source, MessageService.AllTargetsId, contentType, content);
        }

        /// <summary>
        /// 创建消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessage(this IAddinHost host, String destination, ContentType contentType, params Byte[] content)
        {
            if (content == null || content.Length == 0)
            {
                return new Message(destination, MessageService.AddinHostId, ContentType.None, content);
            }
            return new Message(destination, MessageService.AddinHostId, contentType, content);
        }

        /// <summary>
        /// 创建消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessage(this IAddinHost host, String destination, ContentType contentType, String content)
        {
            Byte[] data = Encoding.UTF8.GetBytes(content);
            return CreateMessage(host, destination, contentType, data);
        }

        /// <summary>
        /// 创建发送给所有插件（自己和宿主除外）的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessageToAll(this IAddinHost host, ContentType contentType, params Byte[] content)
        {
            return CreateMessage(host, MessageService.AllTargetsId, contentType, content);
        }

        /// <summary>
        /// 创建发送给所有插件（自己和宿主除外）的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        public static Message CreateMessageToAll(this IAddinHost host, ContentType contentType, String content)
        {
            return CreateMessage(host, MessageService.AllTargetsId, contentType, content);
        }

        /// <summary>
        /// 创建响应消息。
        /// </summary>
        /// <param name="requestMessage">请求消息。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>响应消息。</returns>
        public static Message CreateResponseMessage(this Message requestMessage, ContentType contentType, params Byte[] content)
        {
            if (content == null || content.Length == 0)
            {
                return new Message(requestMessage.Source, requestMessage.Destination, ContentType.None, content);
            }
            return new Message(requestMessage.Source, requestMessage.Destination, contentType, content);
        }

        /// <summary>
        /// 创建响应消息。
        /// </summary>
        /// <param name="requestMessage">请求消息。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>响应消息。</returns>
        public static Message CreateResponseMessage(this Message requestMessage, ContentType contentType, String content)
        {
            Byte[] data = Encoding.UTF8.GetBytes(content);
            return CreateResponseMessage(requestMessage, contentType, data);
        }

        #endregion

        #region Send

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="addin">发送该消息的插件。</param>
        /// <param name="message">消息。</param>
        public static void SendMessage(this IAddin addin, Message message)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (message.Source != addin.Id) throw new InvalidOperationException("Invalid message source");
            if (addin.Proxy == null) throw new ArgumentNullException("Proxy");
            addin.Proxy.Send(message);
        }

        #endregion

        #region Misc

        /// <summary>
        /// 按字符串方式读取消息内容。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <returns></returns>
        public static String ReadAsString(this Message message)
        {
            return Encoding.UTF8.GetString(message.Content);
        }

        #endregion
    }
}
