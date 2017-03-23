using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

        public const String AddinHostId = ".";

        /// <summary>
        /// 表示消息的目标是所有插件（除自己和宿主外）。
        /// </summary>
        public const String AllTargetsId = "*";

        /// <summary>
        /// 注册表。
        /// </summary>
        private readonly Dictionary<String, AddinProxy> _regedit = new Dictionary<String, AddinProxy>();

        /// <summary>
        /// 宿主。
        /// </summary>
        private readonly IAddinHost _host;

        /// <summary>
        /// 为宿主服务的消息调度器。
        /// </summary>
        private readonly MessageDispathcer _messageDispather;

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
            _messageDispather = new MessageDispathcer(_host);
            _messageDispather.Start();
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 在消息服务中注册插件的代理，并启动代理的消息调度器。
        /// </summary>
        /// <param name="proxy">代理。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Register(AddinProxy proxy)
        {
            String id = proxy.Id;
            if (String.IsNullOrWhiteSpace(id)) return false;
            if (_regedit.ContainsKey(id))
            {
                return _regedit[id] == proxy;
            }
            _regedit.Add(id, proxy);
            proxy.MessageService = this;
            proxy.MessageDispather.Start();
            return true;
        }

        /// <summary>
        /// 将代理从消息服务中移除，并释放代理的消息调度器。。
        /// </summary>
        /// <param name="id">所代表对象的Id号。</param>
        public void Unregister(String id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            AddinProxy proxy = _regedit[id];
            _regedit.Remove(id);
            proxy.MessageDispather.Dispose();
            proxy.MessageService = null;
        }

        #endregion

        #region Internal

        /// <summary>
        /// 转发消息。
        /// </summary>
        /// <param name="message">消息。</param>
        internal void OnReceive(Message message)
        {
            Send(message);
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public void Send(Message message)
        {
            String destination = message.Destination;
            if (String.IsNullOrWhiteSpace(destination)) return;
            if (destination == AddinHostId)
            {
                _messageDispather.Queue(message);
            }
            else if (destination == AllTargetsId)
            {
                foreach (String id in _regedit.Keys.Where(x => x != message.Source))
                {
                    _regedit[id].MessageDispather.Queue(message);
                }
            }
            else if (_regedit.ContainsKey(destination))
            {
                _regedit[destination].MessageDispather.Queue(message);
            }
        }

        #endregion

        #endregion
    }
}
