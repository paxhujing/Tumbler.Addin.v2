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
        #region Send

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="service">要发送消息的对象。</param>
        /// <param name="message">消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static void SendMessage(this IMessageSource sender, Message message)
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                MessageService ms = AppDomain.CurrentDomain.GetData("ms") as MessageService;
                ms?.Transmit(message);
            }
            else
            {
                AddinProxy proxy = AppDomain.CurrentDomain.GetData("proxy") as AddinProxy;
                proxy?.Send(message);
            }
        }

        #endregion

        #region Message

        /// <summary>
        /// 创建错误消息。
        /// </summary>
        /// <param name="source">请求消息。</param>
        /// <param name="ex">异常信息。</param>
        /// <returns>错误信息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateErrorMessage(this IMessageSource source, Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");
            return new Message(MessageService.AddinHostId, source.Id, ContentType.None) { Exception = ex };
        }

        #region Request

        /// <summary>
        /// 创建发送给宿主的消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateMessage(this IMessageSource source, ContentType contentType, params Byte[] content)
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
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateMessage(this IMessageSource source, ContentType contentType, String content)
        {
            return CreateMessage(source, MessageService.AddinHostId, contentType, content);
        }

        #region Unicast

        /// <summary>
        /// 创建宿主发送给某个特定目标的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateUnicastMessage(this IAddinHost host, String destination, ContentType contentType, params Byte[] content)
        {
            if (destination == null || destination == MessageService.AllTargetsId)
            {
                throw new InvalidOperationException(destination);
            }
            if (content == null || content.Length == 0)
            {
                return new Message(destination, MessageService.AddinHostId, ContentType.None, content);
            }
            return new Message(destination, MessageService.AddinHostId, contentType, content);
        }

        /// <summary>
        /// 创建宿主发送给某个特定目标的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateUnicastMessage(this IAddinHost host, String destination, ContentType contentType, String content)
        {
            Byte[] data = Encoding.UTF8.GetBytes(content);
            return CreateUnicastMessage(host, destination, contentType, data);
        }

        #endregion

        #region Multicast

        /// <summary>
        /// 创建宿主发送给一组特定目标的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="destinations">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateMulticastMessage(this IAddinHost host, String[] destinations, ContentType contentType, params Byte[] content)
        {
            if (destinations == null || destinations.Length == 0)
            {
                throw new ArgumentException("destinations");
            }
            String temp = String.Join(";", destinations);
            return CreateMessage(host, temp, contentType, content);
        }

        /// <summary>
        /// 创建宿主发送给一组特定目标的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="destinations">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateMulticastMessage(this IAddinHost host, String[] destinations, ContentType contentType, String content)
        {
            Byte[] data = Encoding.UTF8.GetBytes(content);
            return CreateMulticastMessage(host, destinations, contentType, data);
        }

        #endregion

        #region Broadcast

        /// <summary>
        /// 创建发送给所有插件的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateBroadcastMessage(this IAddinHost host, ContentType contentType, params Byte[] content)
        {
            return CreateMessage(host, MessageService.AllTargetsId, contentType, content);
        }

        /// <summary>
        /// 创建发送给所有插件的消息。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateBroadcastMessage(this IAddinHost host, ContentType contentType, String content)
        {
            return CreateMessage(host, MessageService.AllTargetsId, contentType, content);
        }

        #endregion

        #endregion

        #region Response

        /// <summary>
        /// 创建响应消息。
        /// </summary>
        /// <param name="requestMessage">请求消息。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>响应消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateResponseMessage(this Message requestMessage, ContentType contentType, params Byte[] content)
        {
            if (content == null || content.Length == 0)
            {
                return new Message(requestMessage.Source, requestMessage.Destination, ContentType.None, content)
                { IsResponse = true, Id = requestMessage.Id };
            }
            return new Message(requestMessage.Source, requestMessage.Destination, contentType, content)
            { IsResponse = true, Id = requestMessage.Id };
        }

        /// <summary>
        /// 创建响应消息。
        /// </summary>
        /// <param name="requestMessage">请求消息。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>响应消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateResponseMessage(this Message requestMessage, ContentType contentType, String content)
        {
            Byte[] data = Encoding.UTF8.GetBytes(content);
            return CreateResponseMessage(requestMessage, contentType, data);
        }

        /// <summary>
        /// 创建响应失败消息。
        /// </summary>
        /// <param name="requestMessage">请求消息。</param>
        /// <param name="ex">响应失败的异常信息。。</param>
        /// <returns>响应消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateErrorResponseMessage(this Message requestMessage, Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");
            return new Message(requestMessage.Source, requestMessage.Destination, ContentType.None)
            { IsResponse = true, Exception = ex, Id = requestMessage.Id };
        }

        #endregion

        #endregion

        #region Misc

        /// <summary>
        /// 按字符串方式读取消息内容。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <returns></returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static String ReadAsString(this Message message)
        {
            return Encoding.UTF8.GetString(message.Content);
        }

        #endregion

        #region Private

        /// <summary>
        /// 创建消息。
        /// </summary>
        /// <param name="source">消息源。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private static Message CreateMessage(this IMessageSource source, String destination, ContentType contentType, params Byte[] content)
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
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private static Message CreateMessage(this IMessageSource source, String destination, ContentType contentType, String content)
        {
            Byte[] data = Encoding.UTF8.GetBytes(content);
            return CreateMessage(source, destination, contentType, data);
        }

        #endregion
    }
}
