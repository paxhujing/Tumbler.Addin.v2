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
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 在消息服务中注册插件的代理，并启动代理的消息调度器。
        /// </summary>
        /// <param name="target">代理。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Register(IMessageTarget target)
        {
            String id = target.Id;
            if (String.IsNullOrWhiteSpace(id)) return false;
            if (_regedit.ContainsKey(id)) throw new InvalidOperationException($"The id {id} has been Existed");
            AddinProxy proxy = target as AddinProxy;
            if (proxy != null) proxy.MessageService = this;
            _regedit.Add(id, target);
            target.MessageDispatcher?.Start();
            return true;
        }

        /// <summary>
        /// 将代理从消息服务中移除，并释放代理的消息调度器。。
        /// </summary>
        /// <param name="id">所代表对象的Id号。</param>
        public void Unregister(String id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            if (!_regedit.ContainsKey(id)) return;
            IMessageTarget target = _regedit[id];
            target.MessageDispatcher?.Stop();
            AddinProxy proxy = target as AddinProxy;
            if (proxy != null) proxy.MessageService = null;
        }

        /// <summary>
        /// 转发消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public void Transmit(Message message)
        {
            String destination = message.Destination;
            if (String.IsNullOrWhiteSpace(destination)) return;
            if (destination == AddinHostId || destination == _host.Id)
            {
                _host.MessageDispatcher.Queue(message);
            }
            else if (destination == AllTargetsId)
            {
                foreach (IMessageTarget target in _regedit.Values.Where(x => x.Id != message.Source))
                {
                    target.MessageDispatcher.Queue(message);
                }
            }
            else if (_regedit.ContainsKey(destination))
            {
                _regedit[destination].MessageDispatcher.Queue(message);
            }
        }

        #endregion

        #endregion
    }
}
