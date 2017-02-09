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
    public class MessageService : MarshalByRefObject
    {
        #region Fields

        /// <summary>
        /// 注册表。
        /// </summary>
        private readonly Dictionary<String, Delegator> _regedit = new Dictionary<String, Delegator>();

        /// <summary>
        /// 消息格式化器。
        /// </summary>
        private readonly MessageFormatter _formatter = new MessageFormatter();

        private readonly IAddinHost _host;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.MessageService 实例。
        /// </summary>
        /// <param name="host">宿主。</param>
        public MessageService(IAddinHost host)
        {
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
        public Boolean Register(Delegator delegator)
        {
            String id = delegator.ID;
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
        /// <returns>成功返回true；否则返回false。</returns>
        internal Boolean OnReceive(Byte[] data)
        {
            try
            {
                Message message = _formatter.Deserialize(data);
                Delegator delegator = GetDelegator(message.Destination);
                if (delegator == null) return false;

                delegator.OnReceive(data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 转发消息。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        internal Boolean OnReceive(Message message)
        {
            try
            {
                Delegator delegator = GetDelegator(message.Destination);
                if (delegator == null) return false;
                Byte[] data = _formatter.Serialize(message);
                delegator.OnReceive(data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #endregion

        #region Private

        /// <summary>
        /// 获取指定的代表。
        /// </summary>
        /// <param name="destination">所代表对象的ID号。</param>
        /// <returns>要接收消息的代表列表。</returns>
        private Collection<Delegator> GetDelegators(String destination,String source)
        {
            if (String.IsNullOrWhiteSpace(destination)) return null;
            Collection<Delegator> targets = new Collection<Delegator>();
            if (destination == "*")
            {
                foreach (String id in _regedit.Keys.Where(x => x != source))
                {
                    targets.Add(_regedit[id]);
                }
            }
            else if(destination == ". ")
            {

            }
            if (_regedit.ContainsKey(destination))
            {
                return _regedit[destination];
            }
            return targets;
        }

        #endregion

        #endregion
    }
}
