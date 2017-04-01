using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 表示作为其它某个对象的代表。
    /// 代表将代替实际的对象接收或发送消息。
    /// </summary>
    public class Connector : MarshalByRefObject
    {
        #region Fields

        /// <summary>
        /// 所连接的插件。
        /// </summary>
        private readonly IAddin _target;

        /// <summary>
        /// 接收消息队列。
        /// </summary>
        private readonly Queue<Message> _receiveQueue = new Queue<Message>();

        /// <summary>
        /// 消息调度任务。
        /// </summary>
        private readonly Task _messageDispatherTask;

        private readonly AutoResetEvent _sync = new AutoResetEvent(false);

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.Delegator 实例。
        /// </summary>
        /// <param name="target">代表的对象。</param>
        protected Connector(IAddin target)
        {
            if (target == null) throw new ArgumentNullException("target");
            _target = target;
            _messageDispatherTask = new Task(Dispatch, TaskCreationOptions.LongRunning);
            _messageDispatherTask.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 所代表对象的ID号。
        /// </summary>
        public String ID => _target.ID;

        /// <summary>
        /// 消息格式化器。
        /// </summary>
        protected MessageFormatter Formatter { get; } = new MessageFormatter();

        /// <summary>
        /// 消息服务。
        /// </summary>
        internal MessageCenter MessageService { get; set; }

        #endregion

        #region Methods

        #region Internal

        /// <summary>
        /// 让代理将消息发送给所代表的对象。
        /// </summary>
        /// <param name="message">消息的字节表示。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        internal void OnReceive(Byte[] data)
        {
            Message message = Formatter.Deserialize(data);
            _receiveQueue.Enqueue(message);
            _sync.Set();
        }

        /// <summary>
        /// 让代理请求消息服务转发此消息。。
        /// </summary>
        /// <param name="message">消息。</param>
        /// <returns></returns>
        public Boolean OnSend(Message message)
        {
            return MessageService.OnReceive(Formatter.Serialize(message));
        }

        #endregion

        #region Private

        /// <summary>
        /// 调度消息。
        /// </summary>
        private void Dispatch()
        {
            while (_sync.WaitOne())
            {
                Message message = _receiveQueue.Dequeue();
                _target.OnReceive(message);
            }
        }

        #endregion

        #endregion
    }
}
