using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

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
            _parser = new AddinConfigParser(globalConfigFile);
            _messageService = new MessageService(host);
            _loader = GetAddinLoader();
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 获取插件信息。
        /// </summary>
        /// <param name="location">插件配置路径。</param>
        /// <returns>插件信息。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public AddinInfo GetAddinInfo(String location)
        {
            return _parser.GetAddinInfo(location);
        }

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的插件列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<IMessageTarget> LoadAddins(String groupName, Func<IEnumerable<XElement>, IEnumerable<XElement>> filter = null)
        {
            if (String.IsNullOrWhiteSpace(groupName)) return null;
            IEnumerable<XElement> addinNodes = _parser.GetAddinNodes(groupName);
            if (filter != null) addinNodes = filter(addinNodes);
            return LoadAddinImpl(addinNodes, false);
        }

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="subName">插件配置中的子组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的插件列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<IMessageTarget> LoadAddins(String groupName, String subName, Func<IEnumerable<XElement>, IEnumerable<XElement>> filter = null)
        {
            if (String.IsNullOrWhiteSpace(groupName)) return null;
            IEnumerable<XElement> addinNodes = _parser.GetSubAddinNodes(groupName, subName);
            if (filter != null) addinNodes = filter(addinNodes);
            return LoadAddinImpl(addinNodes, false);
        }

        /// <summary>
        /// 加载服务。
        /// </summary>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的服务列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<IMessageTarget> LoadServices(Func<IEnumerable<XElement>, IEnumerable<XElement>> filter = null)
        {
            IEnumerable<XElement> addinNodes = _parser.GetServiceNodes();
            if (filter != null) addinNodes = filter(addinNodes);
            return LoadAddinImpl(addinNodes, true);
        }

        /// <summary>
        /// 卸载插件。
        /// </summary>
        /// <param name="proxy">插件代理。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unload(AddinProxy proxy)
        {
            if (proxy == null) return;
            _messageService.Unregister(proxy.Id);
            _loader.Unload(proxy);
        }

        /// <summary>
        /// 卸载插件。
        /// </summary>
        /// <param name="proxies">一组插件代理。</param>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public void Unload(IEnumerable<AddinProxy> proxies)
        {
            foreach (AddinProxy proxy in proxies)
            {
                Unload(proxy);
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
        /// 获取插件加载器。
        /// </summary>
        /// <returns>插件加载器。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        protected virtual AddinLoader GetAddinLoader()
        {
            return new AddinLoader();
        }

        #endregion

        #region Private

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private IEnumerable<IMessageTarget> LoadAddinImpl(IEnumerable<XElement> addinNodes, Boolean isService)
        {
            Collection<IMessageTarget> proxys = new Collection<IMessageTarget>();
            if (addinNodes != null)
            {
                IMessageTarget temp = null;
                StringBuilder sb = new StringBuilder(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath);
                Collection<XElement> validAddinNodes = new Collection<XElement>();
                foreach (XElement addinNode in addinNodes)
                {
                    UpdatePrivateBinPath(addinNode, ref sb, ref validAddinNodes);
                }
                String privateBinPath = sb.ToString();
                AppDomain.CurrentDomain.SetData("PRIVATE_BINPATH", privateBinPath);
                UpdateContextPropertyMethod.Invoke(null, new Object[] { FunsionHandle, "PRIVATE_BINPATH", privateBinPath });
                foreach (XElement addinNode in validAddinNodes)
                {
                    temp = isService ? _loader.LoadService(addinNode) : _loader.LoadAddin(addinNode);
                    if (temp == null) continue;
                    _messageService.Register(temp);
                    proxys.Add(temp);
                }
            }
            return proxys;
        }

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private void UpdatePrivateBinPath(XElement addinNode, ref StringBuilder sb, ref Collection<XElement> validAddinNodes)
        {
            String location = addinNode.Attribute("location").Value;
            if (Path.IsPathRooted(location)) return;
            String addinConfigFile = Path.Combine(AddinConfigParser.AddinDirectory, location);
            if (!File.Exists(addinConfigFile)) return;
            addinNode.Attribute("location").Value = addinConfigFile;
            AppDomainSetup setup = AppDomain.CurrentDomain.SetupInformation;
            sb.Append($"addins/{Path.GetDirectoryName(location)};");
            validAddinNodes.Add(addinNode);
        }

        #endregion

        #endregion
    }
}
