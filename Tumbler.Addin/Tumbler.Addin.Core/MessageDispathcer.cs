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
    internal class MessageDispathcer : IDisposable
    {
        #region Fields

        /// <summary>
        /// 消息队列。
        /// </summary>
        private readonly Queue<Message> _queue;

        /// <summary>
        /// 消息调度任务。
        /// </summary>
        private readonly Task _messageDispathTask;

        /// <summary>
        /// 同步对象。
        /// </summary>
        private readonly SemaphoreSlim _sync;

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
        public MessageDispathcer(IMessageTarget target)
        {
            if (target == null) throw new ArgumentNullException("target");
            _target = target;
            _sync = new SemaphoreSlim(0, 3);
            _queue = new Queue<Message>();
            _messageDispathTask = new Task(DispathMessage, TaskCreationOptions.LongRunning);
        }

        #endregion

        #region Properties

        /// <summary>
        /// 对象是否已被释放。
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 启动调度器。
        /// </summary>
        public void Start()
        {
            if (_messageDispathTask.Status == TaskStatus.Created)
            {
                _messageDispathTask.Start();
            }
        }

        /// <summary>
        /// 停止并释放占用的资源。
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            _sync.Release();
        }

        /// <summary>
        /// 将消息在调度器中排队等待处理。
        /// </summary>
        /// <param name="message">消息的字节表示。</param>
        internal virtual void Queue(Message message)
        {
            if (IsDisposed) return;
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
            while (!IsDisposed)
            {
                _sync.Wait();
                if (_queue.Count == 0) continue;
                message = _queue.Dequeue();
                _target.OnReceive(message);
            }
            _queue.Clear();
            _sync.Dispose();
        }

        #endregion

        #endregion
    }
}
