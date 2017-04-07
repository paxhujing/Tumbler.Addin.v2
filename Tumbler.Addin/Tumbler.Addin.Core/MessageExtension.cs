﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

        #region Request

        /// <summary>
        /// 创建错误消息。
        /// </summary>
        /// <param name="source">发送者。</param>
        /// <param name="messageCode">消息码。</param>
        /// <param name="ex">异常信息。</param>
        /// <returns>错误信息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateErrorMessage(this IMessageSource source, Int32 messageCode, Exception ex)
        {
            if (ex == null) throw new ArgumentNullException("ex");
            return CreateRequestMessage(source, messageCode, MessageService.AddinHostId, ContentType.Exception, ex);
        }

        /// <summary>
        /// 创建发送给宿主的消息。
        /// </summary>
        /// <param name="source">发送者。</param>
        /// <param name="messageCode">消息码。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateMessageToHost(this IMessageSource source, Int32 messageCode, ContentType contentType, Object content)
        {
            return CreateRequestMessage(source, messageCode, MessageService.AddinHostId, contentType, content);
        }

        /// <summary>
        /// 创建发送给某个特定目标的消息。
        /// </summary>
        /// <param name="source">发送者。</param>
        /// <param name="messageCode">消息码。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateUnicastMessage(this IMessageSource source, Int32 messageCode, String destination, ContentType contentType, Object content)
        {
            if (destination == null || destination == MessageService.AllTargetsId)
            {
                throw new InvalidOperationException(destination);
            }
            return CreateRequestMessage(source, messageCode, destination, contentType, content);
        }

        /// <summary>
        /// 创建发送给一组特定目标的消息。
        /// </summary>
        /// <param name="source">发送者。</param>
        /// <param name="messageCode">消息码。</param>
        /// <param name="destinations">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateMulticastMessage(this IMessageSource source, Int32 messageCode, String[] destinations, ContentType contentType, Object content)
        {
            if (destinations == null || destinations.Length < 2)
            {
                throw new ArgumentException("destinations");
            }
            String temp = String.Join(";", destinations);
            return CreateRequestMessage(source, messageCode, temp, contentType, content);
        }

        /// <summary>
        /// 创建给所有目标的消息。
        /// </summary>
        /// <param name="source">发送者。</param>
        /// <param name="messageCode">消息码。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateBroadcastMessage(this IMessageSource source, Int32 messageCode, ContentType contentType, Object content)
        {
            return CreateRequestMessage(source, messageCode, MessageService.AllTargetsId, contentType, content);
        }

        #endregion

        #region Response

        /// <summary>
        /// 创建响应消息。
        /// </summary>
        /// <param name="requestMessage">请求消息。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public static Message CreateResponseMessage(this Message requestMessage, ContentType contentType, Object content)
        {
            return CreateMessage(requestMessage.MessageCode, requestMessage.Destination, requestMessage.Source, contentType, content, true);
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
            return CreateResponseMessage(requestMessage, ContentType.Exception, ex);
        }

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
            return message.Content as String;
        }

        #endregion

        #region Private

        /// <summary>
        /// 创建请求消息。
        /// </summary>
        /// <param name="source">发送者。</param>
        /// <param name="messageCode">消息码。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private static Message CreateRequestMessage(this IMessageSource source, Int32 messageCode, String destination, ContentType contentType, Object content)
        {
            return CreateMessage(messageCode, source.Id, destination, contentType, content, false);
        }

        /// <summary>
        /// 创建消息。
        /// </summary>
        /// <param name="messageCode">消息码。</param>
        /// <param name="source">发送者。</param>
        /// <param name="destination">目的地。</param>
        /// <param name="contentType">消息内容的类型。</param>
        /// <param name="content">消息内容。</param>
        /// <param name="isResponse">是否是响应消息。</param>
        /// <returns>消息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private static Message CreateMessage(Int32 messageCode, String source, String destination, ContentType contentType, Object content, Boolean isResponse)
        {
            switch (contentType)
            {
                case ContentType.JSON:
                case ContentType.XML:
                case ContentType.Text:
                    if (!(content is String))
                    {
                        throw new ArgumentException("Content is not JSON、XML or Text");
                    }
                    break;
                case ContentType.ByteArray:
                    if (!(content is Byte[]))
                    {
                        throw new ArgumentException("Content is not ByteArray");
                    }
                    break;
                case ContentType.Exception:
                    if (!(content is Exception))
                    {
                        throw new ArgumentException("Content is not Exception");
                    }
                    break;
                case ContentType.Object:
                    if (!AppDomain.CurrentDomain.IsDefaultAppDomain()
                        && !(content is ISerializable)
                        && !(content is MarshalByRefObject))
                    {
                        throw new ArgumentException("Content is not ISerializable or MarshalByRefObject");
                    }
                    break;
                case ContentType.None:
                default:
                    return new Message(messageCode, destination, source, contentType, null) { IsResponse = isResponse };
            }
            return new Message(messageCode, destination, source, contentType, content) { IsResponse = isResponse };
        }

        #endregion
    }
}
