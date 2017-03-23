using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 消息调度器。
    /// </summary>
    public sealed class MessageDispatcher
    {
        #region Fields

        /// <summary>
        /// 消息队列。
        /// </summary>
        private readonly Queue<Message> _queue;

        /// <summary>
        /// 消息调度任务。
        /// </summary>
        private Task _messageDispathTask;

        /// <summary>
        /// 同步对象。
        /// </summary>
        private SemaphoreSlim _sync;

        /// <summary>
        /// 回调方法。
        /// </summary>
        private readonly IMessageTarget _target;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.MessageDispathcer 实例。
        /// </summary>
        /// <param name="callback">回调方法。</param>
        public MessageDispatcher(IMessageTarget target)
        {
            if (target == null) throw new ArgumentNullException("target");
            _target = target;
            _queue = new Queue<Message>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 对象是否已被释放。
        /// </summary>
        public Boolean IsRuning { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动调度器。
        /// </summary>
        internal void Start()
        {
            if (_messageDispathTask == null)
            {
                _sync = new SemaphoreSlim(0, 3);
                _messageDispathTask = new Task(DispathMessage, TaskCreationOptions.LongRunning);
                IsRuning = true;
                _messageDispathTask.Start();
            }
        }

        /// <summary>
        /// 停止调度器。
        /// </summary>
        internal void Stop()
        {
            if (_messageDispathTask != null && _messageDispathTask.Status == TaskStatus.Running)
            {
                IsRuning = false;
                _sync.Release();
            }
        }

        /// <summary>
        /// 将消息在调度器中排队等待处理。
        /// </summary>
        /// <param name="message">消息的字节表示。</param>
        internal void Queue(Message message)
        {
            if (!IsRuning) return;
            _queue.Enqueue(message);
            _sync.Release();
        }

        #endregion

        #region Private

        /// <summary>
        /// 分发消息。
        /// </summary>
        private void DispathMessage()
        {
            Message message;
            while (IsRuning)
            {
                _sync.Wait();
                if (_queue.Count == 0) continue;
                message = _queue.Dequeue();
                _target.OnReceive(message);
            }
            _messageDispathTask = null;
            _queue.Clear();
            _sync.Dispose();
            _sync = null;
        }

        #endregion

        #endregion
    }
}
