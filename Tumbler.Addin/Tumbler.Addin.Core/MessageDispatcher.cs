using System;
using System.Collections.Concurrent;
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
    public sealed class MessageDispatcher : MarshalByRefObject
    {
        #region Fields

        /// <summary>
        /// 消息队列。
        /// </summary>
        private readonly ConcurrentQueue<Message> _queue;

        /// <summary>
        /// 消息调度任务。
        /// </summary>
        private Task _messageDispathTask;

        /// <summary>
        /// 回调方法。
        /// </summary>
        private readonly IMessageTarget _target;

        private static Int32 MaxCount = 10;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.MessageDispathcer 实例。
        /// </summary>
        /// <param name="target">实现了 IMessageTarget 接口的类型实例。</param>
        public MessageDispatcher(IMessageTarget target)
        {
            if (target == null) throw new ArgumentNullException("target");
            _target = target;
            _queue = new ConcurrentQueue<Message>();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 对象是否已被释放。
        /// </summary>
        public Boolean IsRuning { get; private set; }

        #endregion

        #region Methods

        #region Internal

        /// <summary>
        /// 启动调度器。
        /// </summary>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal void Start()
        {
            if (_messageDispathTask == null)
            {
                _messageDispathTask = new Task(DispathMessage, TaskCreationOptions.LongRunning);
                IsRuning = true;
                _messageDispathTask.Start();
            }
        }

        /// <summary>
        /// 停止调度器。
        /// </summary>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal void Stop()
        {
            if (_messageDispathTask != null && _messageDispathTask.Status == TaskStatus.Running)
            {
                IsRuning = false;
            }
        }

        /// <summary>
        /// 将消息在调度器中排队等待处理。
        /// </summary>
        /// <param name="message">消息的字节表示。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        internal void Queue(Message message)
        {
            if (!IsRuning) return;
            if (_queue.Count == MaxCount)
            {
                System.Diagnostics.Debug.WriteLine("Message queue full");
                return;
            }
            _queue.Enqueue(message);
        }

        #endregion

        #region Private

        /// <summary>
        /// 分发消息。
        /// </summary>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void DispathMessage()
        {
            Message message;
            while (IsRuning)
            {
                System.Diagnostics.Debug.WriteLine("message queue count: {0}", _queue.Count);
                if (_queue.Count == 0)
                {
                    Thread.Sleep(100);
                }
                if (_queue.TryDequeue(out message))
                {
                    _target.OnReceive(message);
                }
            }
            _messageDispathTask = null;
        }

        #endregion

        #endregion
    }
}
