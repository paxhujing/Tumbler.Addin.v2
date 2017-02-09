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
    public class Delegator : MarshalByRefObject
    {
        #region Fields

        private readonly IAddin _target;

        private readonly ConcurrentQueue<Message> _queue = new ConcurrentQueue<Message>();

        private readonly Task _processTask;

        private readonly SemaphoreSlim _sync = new SemaphoreSlim(0, 3);

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.Delegator 实例。
        /// </summary>
        /// <param name="target">代表的对象。</param>
        internal protected Delegator(IAddin target)
        {
            if (target == null) throw new ArgumentNullException("target");
            _target = target;
            _processTask = new Task(Process, TaskCreationOptions.LongRunning);
            _processTask.Start();
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
        internal MessageService MessageService { get; set; }

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
            _queue.Enqueue(message);
            _sync.Release();
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

        private void Process()
        {
            Message message;
            while (true)
            {
                if (_queue.TryDequeue(out message))
                {
                    _target.OnReceive(message);
                }
                else
                {
                    _sync.Wait();
                }
            }
        }

        #endregion

        #endregion
    }
}
