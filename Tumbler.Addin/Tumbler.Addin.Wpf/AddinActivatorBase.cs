using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Tumbler.Addin.Core;

namespace Tumbler.Addin.Wpf
{
    /// <summary>
    /// 插件激活器。
    /// </summary>
    public abstract class AddinActivatorBase : INotifyPropertyChanged, IMessageSource
    {
        #region Event

        #region PropertyChanged

        /// <summary>
        /// 属性改变事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 触发属性改变事件。
        /// </summary>
        /// <param name="propertyName">属性名称。</param>
        protected void OnPropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region Launched

        /// <summary>
        /// 插件已启动事件。
        /// </summary>
        public event EventHandler Launched;

        /// <summary>
        /// 触发插件启动事件。
        /// </summary>
        private void OnLaunched()
        {
            if (Launched != null)
            {
                Launched(this, EventArgs.Empty);
            }
        }

        #endregion

        #region  Closed

        /// <summary>
        /// 插件已关闭事件。
        /// </summary>
        public event EventHandler Closed;

        /// <summary>
        /// 触发插件关闭事件。
        /// </summary>
        private void OnClosed()
        {
            if (Closed != null)
            {
                Closed(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Actived 

        /// <summary>
        /// 插件已激活事件。
        /// </summary>
        public event EventHandler Actived;

        /// <summary>
        /// 触发插件激活事件。
        /// </summary>
        private void OnActived()
        {
            if (Actived != null)
            {
                Actived(this, EventArgs.Empty);
            }
        }

        #endregion

        #region Inactived 

        /// <summary>
        /// 插件未激活事件。
        /// </summary>
        public event EventHandler Inactived;

        /// <summary>
        /// 触发插件未激活事件。
        /// </summary>
        private void OnInactived()
        {
            if (Inactived != null)
            {
                Inactived(this, EventArgs.Empty);
            }
        }

        #endregion

        #endregion

        #region Fields

        internal WpfAddinManager _addinManager;

        internal WpfAddinInfo _info;

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Wpf.AddinActivatorBase 实例。
        /// </summary>
        protected AddinActivatorBase()
        {

        }

        #endregion

        #region Properties

        /// <summary>
        /// 插件管理器。
        /// </summary>
        protected WpfAddinManager AddinManager => _addinManager;

        /// <summary>
        /// 内部消息收发器。
        /// </summary>
        public InternalMessageTransceiver Transceiver => (Addin as WpfAddinProxy)?.Transceiver;

        /// <summary>
        /// 插件信息。
        /// </summary>
        public WpfAddinInfo Info => _info;

        /// <summary>
        /// 插件。
        /// </summary>
        protected IAddin Addin { get; private set; }

        /// <summary>
        /// 插件的视图。
        /// </summary>
        public FrameworkElement View { get; private set; }

        /// <summary>
        /// 激活器的Id。
        /// </summary>
        public String Id
        {
            get
            {
                if (!IsActived) throw new InvalidOperationException("The activator need be actived");
                return Addin.Id;
            }
        }

        #region IsActived

        private Boolean _isActived;
        /// <summary>
        /// 是否已被激活。
        /// </summary>
        public Boolean IsActived
        {
            get { return _isActived; }
            private set
            {
                _isActived = value;
                OnPropertyChanged("IsActived");
            }
        }

        #endregion

        #region IsLaunched

        private Boolean _isLaunched;
        /// <summary>
        /// 是否被启动。
        /// </summary>
        public Boolean IsLaunched
        {
            get { return _isLaunched; }
            private set
            {
                _isLaunched = value;
                OnPropertyChanged("IsLaunched");
            }
        }

        #endregion

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 激活插件。
        /// </summary>
        public void Active()
        {
            if (IsActived) return;
            IAddin addin = AddinManager.LoadAddin(_info);
            if (addin != null)
            {
                FrameworkElement view = CreateView(addin);
                if (view != null)
                {
                    view.Tag = this;
                    View = view;
                }
                Addin = addin;
                WpfAddinProxy proxy = addin as WpfAddinProxy;
                if (proxy != null)
                {
                    proxy.Transceiver = new InternalMessageTransceiver(proxy);
                }
            }
            if (addin != null)
            {
                IsActived = true;
                OnActived();
            }
            else
            {
                IsActived = false;
            }
        }

        /// <summary>
        /// 询问激活区需要的启动条件。
        /// </summary>
        /// <returns>启动条件列表。</returns>
        public virtual IEnumerable<ILaunchCondition> AskLaunchConditions()
        {
            return null;
        }

        /// <summary>
        /// 启动插件。
        /// </summary>
        public void Launch()
        {
            if (!IsActived) throw new InvalidOperationException("The activator need be actived");
            IsLaunched = LaunchCore();
            OnLaunched();
        }

        /// <summary>
        /// 关闭插件。
        /// </summary>
        public void Close()
        {
            if (!IsActived) throw new InvalidOperationException("The activator need be actived");
            IsLaunched = false;
            Cleanup();
            OnClosed();
        }

        /// <summary>
        /// 使插件变为非激活状态。
        /// </summary>
        public void Inactive()
        {
            if (!IsActived) throw new InvalidOperationException("The activator need be actived");
            IsLaunched = false;
            IsActived = false;
            Cleanup();
            AddinManager.Unload(Addin);
            OnInactived();
        }

        #endregion

        #region Procatectd

        /// <summary>
        /// 创建插件的视图。
        /// </summary>
        /// <param name="addin">插件。</param>
        /// <returns>插件的视图。</returns>
        protected abstract FrameworkElement CreateView(IAddin addin);

        /// <summary>
        /// 启动插件。
        /// </summary>
        /// <returns>启动成功返回true，否则返回false。</returns>
        protected abstract Boolean LaunchCore();

        /// <summary>
        /// 清理资源。
        /// </summary>
        protected virtual void Cleanup()
        {
        }

        #endregion

        #endregion
    }
}
