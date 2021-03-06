﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;
using System.Xml.Schema;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件管理器。
    /// </summary>
    public class AddinManager
    {
        #region Fields

        /// <summary>
        /// 消息服务。
        /// </summary>
        private readonly MessageService _messageService;

        /// <summary>
        /// 配置解析器。
        /// </summary>
        private readonly AddinConfigParser _parser;

        /// <summary>
        /// 加载器。
        /// </summary>
        private readonly AddinLoader _loader;

        private static readonly MethodInfo UpdateContextPropertyMethod;

        private static readonly Object FunsionHandle;

        private readonly Collection<String> _privateBinPathCache = new Collection<String>();

        #endregion

        #region Constructors

        static AddinManager()
        {
            FunsionHandle = typeof(AppDomain).GetMethod("GetFusionContext", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(AppDomain.CurrentDomain, null);
            UpdateContextPropertyMethod = typeof(AppDomainSetup).GetMethod("UpdateContextProperty", BindingFlags.NonPublic | BindingFlags.Static);
        }

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinManager 实例。
        /// </summary>
        /// <param name="host">宿主。</param>
        /// <param name="globalConfigFile">全局配置文件。</param>
        public AddinManager(IAddinHost host, String globalConfigFile)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (String.IsNullOrWhiteSpace(globalConfigFile))
            {
                throw new ArgumentNullException("globalConfigFile");
            }
            if (!Path.IsPathRooted(globalConfigFile))
            {
                globalConfigFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, globalConfigFile);
            }
            if (!File.Exists(globalConfigFile))
            {
                throw new FileNotFoundException(globalConfigFile);
            }
            _parser = CreateAddinConfigParser(globalConfigFile);
            _messageService = new MessageService(host);
            _loader = CreateAddinLoader();
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取所有插件组的名称。
        /// </summary>
        /// <returns>插件组的名称列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<String> GetAddinGroupNames()
        {
            return _parser.GetAddinGroupNames();
        }

        /// <summary>
        /// 获取插件组的所有子组名称。
        /// </summary>
        /// <returns>插件组的名称列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<String> GetSubNames(String groupName)
        {
            return _parser.GetSubNames(groupName);
        }

        /// <summary>
        /// 获取插件信息。
        /// </summary>
        /// <param name="location">插件配置路径。</param>
        /// <returns>插件信息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public AddinInfo GetAddinInfo(String location)
        {
            return _parser.GetAddinInfo(location, false);
        }

        /// <summary>
        /// 获取插件信息。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>插件信息列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinInfo> GetAddinInfos(String groupName, Func<IEnumerable<AddinInfo>, IEnumerable<AddinInfo>> filter = null)
        {
            if (String.IsNullOrWhiteSpace(groupName)) return null;
            IEnumerable<AddinInfo> infos = _parser.GetAddinInfos(groupName);
            if (filter != null) infos = filter(infos);
            return infos;
        }

        /// <summary>
        /// 获取插件信息。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="subName">插件配置中的子组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>插件信息列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinInfo> GetAddinInfos(String groupName,String subName, Func<IEnumerable<AddinInfo>, IEnumerable<AddinInfo>> filter = null)
        {
            if (String.IsNullOrWhiteSpace(groupName) || String.IsNullOrWhiteSpace(subName)) return null;
            IEnumerable<AddinInfo> infos = _parser.GetSubAddinInfos(groupName, subName);
            if (filter != null) infos = filter(infos);
            return infos;
        }

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="info">插件信息。</param>
        /// <returns>如果成功则不为null。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IAddin LoadAddin(AddinInfo info)
        {
            return LoadAddinImpl(new AddinInfo[] { info }, false).FirstOrDefault();
        }

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的插件列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<IAddin> LoadAddins(String groupName, Func<IEnumerable<AddinInfo>, IEnumerable<AddinInfo>> filter = null)
        {
            return LoadAddinImpl(GetAddinInfos(groupName, filter), false);
        }

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="subName">插件配置中的子组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的插件列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<IAddin> LoadAddins(String groupName, String subName, Func<IEnumerable<AddinInfo>, IEnumerable<AddinInfo>> filter = null)
        {
            return LoadAddinImpl(GetAddinInfos(groupName, subName, filter), false);
        }

        /// <summary>
        /// 加载服务插件。
        /// </summary>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的服务插件列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<IService> LoadServices(Func<IEnumerable<AddinInfo>, IEnumerable<AddinInfo>> filter = null)
        {
            IEnumerable<AddinInfo> infos = _parser.GetServiceInfos();
            if (filter != null) infos = filter(infos);
            return LoadAddinImpl(infos, true)?.Cast<IService>();
        }
        /// <summary>
        /// 卸载插件。
        /// </summary>
        /// <param name="addin">插件。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unload(IAddin addin)
        {
            if (addin == null) return;
            _messageService.Unregister(addin.Id);
            _loader.Unload(addin);
            addin.Destroy();
        }

        /// <summary>
        /// 卸载插件。
        /// </summary>
        /// <param name="addins">一组插件。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unload(IEnumerable<IAddin> addins)
        {
            foreach (IAddin addin in addins)
            {
                Unload(addin);
            }
        }

        /// <summary>
        /// 将代理从消息服务中移除，并释放代理的消息调度器。。
        /// </summary>
        /// <param name="id">所代表对象的Id号。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unregister(String id)
        {
            _messageService.Unregister(id);
        }

        /// <summary>
        /// 在消息服务中注册插件的代理，并启动代理的消息调度器。
        /// </summary>
        /// <param name="target">代理。</param>
        /// <returns>成功返回true；否则返回false。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Register(IMessageTarget target)
        {
            _messageService.Register(target);
        }

        /// <summary>
        /// 终止消息服务。
        /// </summary>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Terminate()
        {
            _messageService.Terminate();
        }

        #endregion

        #region Protected

        /// <summary>
        /// 创建插件加载器。
        /// </summary>
        /// <returns>插件加载器。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual AddinLoader CreateAddinLoader()
        {
            return new AddinLoader();
        }

        /// <summary>
        /// 创建插件解析器。
        /// </summary>
        /// <param name="globalConfigFile">插件全局配置文件。</param>
        /// <returns>插件解析器。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual AddinConfigParser CreateAddinConfigParser(String globalConfigFile)
        {
            return new AddinConfigParser(globalConfigFile);
        }

        /// <summary>
        /// 添加程序私有目录。
        /// </summary>
        /// <param name="path">私有目录。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected void AddPrivateBinPath(String path)
        {
            if (_privateBinPathCache.Contains(path)) return;
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            String privateBinPath = setup.PrivateBinPath + path + ";";
            AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", privateBinPath);
            UpdateContextPropertyMethod.Invoke(null, new Object[] { FunsionHandle, "PRIVATE_BINPATH", privateBinPath });
            _privateBinPathCache.Add(path);
        }

        #endregion

        #region Private

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private IEnumerable<IAddin> LoadAddinImpl(IEnumerable<AddinInfo> infos, Boolean isService)
        {
            Collection<IAddin> addins = new Collection<IAddin>();
            if (infos != null && infos.Count() != 0)
            {
                IAddin temp = null;
                foreach (AddinInfo info in infos)
                {
                    AddPrivateBinPath($"addins/{Path.GetDirectoryName(info.Location)}");
                }
                foreach (AddinInfo info in infos)
                {
                    temp = isService ? _loader.LoadService(info) : _loader.LoadAddin(info);
                    if (temp == null) continue;
                    _messageService.Register(temp);
                    addins.Add(temp);
                    temp.Initialize();
                }
            }
            return addins;
        }

        #endregion

        #endregion
    }
}
