using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Sun.Core.Plugin;
using Sun.Core.Ioc;
using Sun.Core.Caching;
using Autofac;

namespace Plugin.RedisCache
{
    [IocExport(typeof(IPlugin),ContractName = "10d97180-ce62-4dc5-b6ab-77d8601ab12a", SingleInstance = true)]
    public class RedisPlugin : IPlugin
    {
        string guid = "10d97180-ce62-4dc5-b6ab-77d8601ab12a";
        public string Name
        {
            get { return "RedisCache"; }
        }

		private PluginInfo pluginInfo;
        public void Initialize(PluginInfo info) 
        {
            pluginInfo = info;
            var hostList = JsonConvert.DeserializeObject<List<ConfigInfo>>(info.Config);
            foreach (var host in hostList)
            {
                if (host.Name == "datas")
                    WebIoc.Register(b => b.Register<RedisCache>(c => new RedisCache(host)).As<ICache>().SingleInstance());
                WebIoc.Register(b => b.Register<RedisCache>(c => new RedisCache(host)).Named<ICache>(host.Name).SingleInstance(), guid, info.Status == PluginStatus.Usable);
            }
            WebIoc.Register(b => b.Register(c => new Session(c.ResolveNamed<ICache>("session"))).As<Sun.Core.Session.ISession>().PreserveExistingDefaults().SingleInstance(), guid, info.Status == PluginStatus.Usable);
        } 
        public void Stop()
        {
            IFileOperate parser = WebIoc.Container.Resolve<IFileOperate>();
            pluginInfo.Status = PluginStatus.Stop;
            parser.SavePluginInfo(pluginInfo);
            WebIoc.Instance.StopAssembly(guid);
        }

        public void Start()
        {
            IFileOperate parser = WebIoc.Container.Resolve<IFileOperate>();
            pluginInfo.Status = PluginStatus.Usable;
            parser.SavePluginInfo(pluginInfo);
            WebIoc.Register(b => b.Register(c => new Session(c.ResolveNamed<ICache>("session"))).As<Sun.Core.Session.ISession>().SingleInstance(),guid);
            WebIoc.Instance.StartAssembly(guid);
        }


        public PluginInfo Info
        {
            get { if (null == pluginInfo) throw new Exception("插件未进行初始化"); return pluginInfo; }
        }
    }
}
