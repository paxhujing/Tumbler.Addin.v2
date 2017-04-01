using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Tumbler.Addin.Core
{
    /// <summary>
    /// 插件管理器。
    /// </summary>
    public sealed class AddinManager
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

        #endregion

        #region Constructors

        /// <summary>
        /// 初始化类型 Tumbler.Addin.Core.AddinManager 实例。
        /// </summary>
        /// <param name="host"></param>
        /// <param name="globalConfigFile"></param>
        public AddinManager(IAddinHost host, String globalConfigFile)
        {
            if (host == null) throw new ArgumentNullException("host");
            if (host.MessageService == null) throw new ArgumentNullException("host.MessageService");
            if (String.IsNullOrWhiteSpace(globalConfigFile)) throw new ArgumentNullException("globalConfigFile");
            _parser = new AddinConfigParser(globalConfigFile);
            _messageService = host.MessageService;
            _loader = new AddinLoader();
        }

        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的插件列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinProxy> LoadAddins(String groupName, Func<IEnumerable<XElement>, IEnumerable<XElement>> filter = null)
        {
            if (String.IsNullOrWhiteSpace(groupName)) return null;
            IEnumerable<XElement> addinNodes = _parser.GetAddinNodes(groupName);
            if (filter != null) addinNodes = filter(addinNodes);
            return LoadAddinImpl(addinNodes);
        }

        /// <summary>
        /// 加载插件。
        /// </summary>
        /// <param name="groupName">插件配置中的组名称。</param>
        /// <param name="subName">插件配置中的子组名称。</param>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的插件列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinProxy> LoadAddins(String groupName, String subName, Func<IEnumerable<XElement>, IEnumerable<XElement>> filter = null)
        {
            if (String.IsNullOrWhiteSpace(groupName)) return null;
            IEnumerable<XElement> addinNodes = _parser.GetSubAddinNodes(groupName, subName);
            if (filter != null) addinNodes = filter(addinNodes);
            return LoadAddinImpl(addinNodes);
        }

        /// <summary>
        /// 加载服务。
        /// </summary>
        /// <param name="filter">过滤器。用于筛选出需要加载的插件。</param>
        /// <returns>加载成功的服务列表。</returns>
        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        public IEnumerable<AddinProxy> LoadServices(Func<IEnumerable<XElement>, IEnumerable<XElement>> filter = null)
        {
            IEnumerable<XElement> addinNodes = _parser.GetServiceNodes();
            if (filter != null) addinNodes = filter(addinNodes);
            return LoadAddinImpl(addinNodes);
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

        #endregion

        #region Private

        [LoaderOptimization(LoaderOptimization.MultiDomain)]
        private IEnumerable<AddinProxy> LoadAddinImpl(IEnumerable<XElement> addinNodes)
        {
            Collection<AddinProxy> proxys = new Collection<AddinProxy>();
            if (addinNodes != null)
            {
                AddinProxy temp = null;
                foreach (XElement addinNode in addinNodes)
                {
                    temp = _loader.LoadAddin(addinNode);
                    if (temp == null) continue;
                    _messageService.Register(temp);
                    proxys.Add(temp);
                }
            }
            return proxys;
        }

        #endregion

        #endregion
    }
}
