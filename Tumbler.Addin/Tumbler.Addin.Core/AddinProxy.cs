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
    /// 插件代理。用于插件与宿主之间的通信。
    /// </summary>
    public class AddinProxy : MarshalByRefObject, IDisposable
    {
        #region Fields

        /// <summary>
        /// 与该代理关联的插件。
        /// </summary>
        private readonly IAddin _target;

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

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.Delegator 实例。
        /// </summary>
        /// <param name="target">代表的对象。</param>
        protected AddinProxy(IAddin target)
            : this()
        {
            if (target == null) throw new ArgumentNullException("target");
            _target = target;
        }

        internal AddinProxy()
        {
            _sync = new SemaphoreSlim(0, 3);
            _queue = new Queue<Message>();
            _messageDispathTask = new Task(DispathMessage, TaskCreationOptions.LongRunning);
            _messageDispathTask.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// 所关联插件的ID号。
        /// </summary>
        public string Id => _target.Id;

        /// <summary>
        /// 对象是否已被释放。
        /// </summary>
        public Boolean IsDisposed { get; private set; }

        /// <summary>
        /// 消息服务。
        /// </summary>
        internal MessageService MessageService { get; set; }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 释放占用的资源。
        /// </summary>
        public void Dispose()
        {
            if (IsDisposed) return;
            IsDisposed = true;
            _sync.Release();
        }

        #endregion

        #region Internal

        /// <summary>
        /// 让代理将消息发送给所代表的对象。
        /// </summary>
        /// <param name="message">消息的字节表示。</param>
        internal virtual void OnReceive(Message message)
        {
            if (IsDisposed) return;
            _queue.Enqueue(message);
            _sync.Release();
        }

        /// <summary>
        /// 让代理请求消息服务转发此消息。。
        /// </summary>
        /// <param name="message">消息。</param>
        public virtual void Send(Message message)
        {
            if (IsDisposed) return;
            MessageService.OnReceive(message);
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
