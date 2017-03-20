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
    public class MessageService : AddinProxy
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
        private readonly Dictionary<String, AddinProxy> _regedit = new Dictionary<String, AddinProxy>();

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
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 在消息服务中注册一位代表。
        /// </summary>
        /// <param name="delegator">代表。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        public Boolean Register(AddinProxy delegator)
        {
            String id = delegator.Id;
            if (String.IsNullOrWhiteSpace(id)) return false;
            if (_regedit.ContainsKey(id))
            {
                try
                {
                    return _regedit[id] == delegator;
                }//对象已失效
                catch(RemotingException)
                {
                    _regedit.Remove(id);
                    _regedit.Add(id, delegator);
                    return true;
                }
            }
            _regedit.Add(id, delegator);
            return true;
        }

        /// <summary>
        /// 将代表从消息服务中移除。
        /// </summary>
        /// <param name="id">所代表对象的ID号。</param>
        public void Unregister(String id)
        {
            if (String.IsNullOrWhiteSpace(id)) return;
            _regedit.Remove(id);
        }

        /// <summary>
        /// 转发消息。
        /// </summary>
        /// <param name="message">消息。</param>
        internal override void OnReceive(Message message)
        {
            Send(message);
        }

        /// <summary>
        /// 发送消息。
        /// </summary>
        /// <param name="message">消息。</param>
        public override void Send(Message message)
        {
            String destination = message.Destination;
            if (String.IsNullOrWhiteSpace(destination)) return;
            if (destination == AddinHostId)
            {
                _host.OnReceive(message);
            }
            else if (destination == AllTargetsId)
            {
                foreach (String id in _regedit.Keys.Where(x => x != message.Source))
                {
                    _regedit[id].OnReceive(message);
                }
            }
            else if (_regedit.ContainsKey(destination))
            {
                _regedit[destination].OnReceive(message);
            }
        }

        #endregion

        #endregion
    }
}
