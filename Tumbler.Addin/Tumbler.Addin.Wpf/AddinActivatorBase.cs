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

        #endregion

        #region Fields

        internal WpfAddinManager _addinManager;

        internal WpfAddinInfo _info;

        private IWpfAddin _addin;

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
        public InternalMessageTransceiver Transceiver => (_addin as WpfAddinProxy)?.Transceiver;

        /// <summary>
        /// 插件信息。
        /// </summary>
        public WpfAddinInfo Info => _info;

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
                return _addin.Id;
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
                IWpfAddin wpfAddin = addin as IWpfAddin;
                if (wpfAddin == null)
                {
                    AddinManager.Unload(addin);
                }
                else
                {
                    FrameworkElement view = CreateView();
                    if (view == null)
                    {
                        AddinManager.Unload(addin);
                    }
                    else
                    {
                        view.Tag = this;
                        View = view;
                        _addin = wpfAddin;
                        WpfAddinProxy proxy = addin as WpfAddinProxy;
                        if (proxy != null)
                        {
                            proxy.Transceiver = new InternalMessageTransceiver(proxy);
                        }
                    }
                }
            }
            IsActived = _addin != null;
        }

        /// <summary>
        /// 启动插件。
        /// </summary>
        public void Launch()
        {
            if (!IsActived) throw new InvalidOperationException("The activator need be actived");
            IsLaunched = LaunchCore(_addin);
        }

        /// <summary>
        /// 关闭插件。
        /// </summary>
        public void Close()
        {
            if (!IsActived) throw new InvalidOperationException("The activator need be actived");
            if(CloseCore())
            {
                Cleanup(false);
            }
        }

        /// <summary>
        /// 清理资源。
        /// </summary>
        /// <param name="isInactivate">是否是插件非激活时的清理操作。</param>
        public virtual void Cleanup(Boolean isInactivate)
        {
            IsLaunched = false;
            if (isInactivate) IsActived = false;
        }

        /// <summary>
        /// 使插件变为非激活状态。
        /// </summary>
        public void Inactive()
        {
            if (!IsActived) throw new InvalidOperationException("The activator need be actived");
            if (InactiveCore())
            {
                Cleanup(true);
            }
        }

        #endregion

        #region Procatectd

        /// <summary>
        /// 使插件变为未激活时的状态。
        /// </summary>
        protected abstract Boolean InactiveCore();

        /// <summary>
        /// 创建插件的视图。
        /// </summary>
        /// <returns>插件的视图。</returns>
        protected abstract FrameworkElement CreateView();

        /// <summary>
        /// 启动插件。
        /// </summary>
        /// <param name="addin">插件。</param>
        /// <returns>启动成功返回true，否则返回false。</returns>
        protected abstract Boolean LaunchCore(IWpfAddin addin);

        /// <summary>
        /// 关闭插件。
        /// </summary>
        /// <returns>终止成功返回true，否则返回false。</returns>
        protected abstract Boolean CloseCore();

        #endregion

        #endregion
    }
}
