using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 消息服务，用于转发消息。
    /// </summary>
    public sealed class MessageService : MarshalByRefObject
    {
        #region Fields

        /// <summary>
        /// 表示消息的目标是宿主。
        /// </summary>
        public const String AddinHostId = ".";

        /// <summary>
        /// 表示消息的目标是所有插件（除自己和宿主外）。
        /// </summary>
        public const String AllTargetsId = "*";

        /// <summary>
        /// 注册表。
        /// </summary>
        private readonly Dictionary<String, IMessageTarget> _regedit = new Dictionary<String, IMessageTarget>();

        /// <summary>
        /// 宿主。
        /// </summary>
        private readonly IAddinHost _host;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.MessageService 实例。
        /// </summary>
        /// <param name="host">宿主。</param>
        public MessageService(IAddinHost host)
        {
            if(host == null) throw new ArgumentNullException("host");
            _host = host;
            host.MessageDispatcher.Start();
            AppDomain.CurrentDomain.SetData("ms", this);
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取指定的消息目标对象。
        /// </summary>
        /// <param name="id">消息目标的id号。</param>
        /// <returns>IMessageTarget 类型实例。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IMessageTarget GetTarget(String id)
        {
            lock (_regedit)
            {
                if (_regedit.ContainsKey(id))
                {
                    return _regedit[id];
                }
            }
            return null;
        }

        /// <summary>
        /// 在消息服务中注册插件的代理，并启动代理的消息调度器。
        /// </summary>
        /// <param name="target">代理。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public Boolean Register(IMessageTarget target)
        {
            String id = target.Id;
            if (String.IsNullOrWhiteSpace(id)) return false;
            lock (_regedit)
            {
                if (_regedit.ContainsKey(id)) throw new InvalidOperationException($"The id {id} has been Existed");
                _regedit.Add(id, target);
            }
            AddinProxy proxy = target as AddinProxy;
            if (proxy != null)
            {
                proxy.MessageService = this;
                proxy.Owner.DomainUnload += OnDomainUnload;
            }
            target.MessageDispatcher?.Start();
#if DEBUG
            Console.WriteLine($"Register target.{target.Id}");
#endif
            return true;
        }

        /// <summary>
        /// 将代理从消息服务中移除，并释放代理的消息调度器。。
        /// </summary>
        /// <param name="id">所代表对象的Id号。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unregister(String id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            IMessageTarget target = null;
            lock (_regedit)
            {
                if (!_regedit.ContainsKey(id)) return;
                target = _regedit[id];
                _regedit.Remove(id);
            }
            target.MessageDispatcher?.Stop();
            AddinProxy proxy = target as AddinProxy;
            if (proxy != null) proxy.MessageService = null;
#if DEBUG
            Console.WriteLine($"Unregister target.{target.Id}");
#endif
        }

        /// <summary>
        /// 转发消息。
        /// </summary>
        /// <param name="message">消息。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Transmit(Message message)
        {
            String destination = message.Destination;
            if (String.IsNullOrWhiteSpace(destination)) return;
            if (destination == AddinHostId || destination == _host.Id)
            {
                _host.MessageDispatcher.Queue(message);
            }
            else
            {
                TransmitSpecialMessage(message);
            }
        }

        #endregion

        #region Private

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void TransmitSpecialMessage(Message message)
        {
            if (message.Destination == AllTargetsId)
            {
                TransmitSameMessageToSomeone(message, _regedit.Keys.ToArray());
                message.Destination = AllTargetsId;
            }
            else
            {
                String temp = message.Destination;
                String[] destinations = message.Destination.Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                TransmitSameMessageToSomeone(message, destinations);
                message.Destination = temp;
            }
        }

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void TransmitSameMessageToSomeone(Message message, params String[] destinations)
        {
            lock (_regedit)
            {
                foreach (String destination in destinations)
                {
                    if (!_regedit.ContainsKey(destination)) continue;
                    message.Destination = destination;
#if DEBUG
                    Console.WriteLine($"[{message.Id}]Transmit from {message.Source} to {message.Destination}");
                    Console.WriteLine($"\tIsErrorMessage:{message.IsFailed}");
                    Console.WriteLine($"\tContentType:{message.ContentType}");
                    Console.WriteLine($"\tContentLength:{message.Content.Length}");
#endif

                    if (_regedit[destination] is AddinProxy)
                    {
                        _regedit[destination].MessageDispatcher.Queue(message);
                    }
                    else
                    {
                        _regedit[destination].MessageDispatcher.Queue((Message)message.Clone());
                    }
                }
            }
        }

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void OnDomainUnload(object sender, EventArgs e)
        {
            AppDomain domain = (AppDomain)sender;
            String id = domain.GetData("id") as String;
            Unregister(id);
        }

        #endregion

        #endregion
    }
}
