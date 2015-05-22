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
using Plugin.SunLog.DB;
using Sun.Core.Logging;


namespace Plugin.SunLog
{
    [IocExport(typeof(IPlugin), ContractName = "f6fc61b5-2314-4149-aebb-3de0eb83221e", SingleInstance = true)]
    public class SunLogPlugin : IPlugin
    {
        string assemblyGuid = "f6fc61b5-2314-4149-aebb-3de0eb83221e";
         public ILogOperate Operate
        {
            get;
            set;
        }
		public SunLogPlugin(ILogOperate operate)
		{
			Operate = operate;
		}
        public string Name
        {
            get { return "sunlog"; }
        }

	    internal static ConfigInfo PluginConfig;
        static PluginInfo pluginInfo;
        public void Initialize(PluginInfo info)
        {
            pluginInfo = info;
            PluginConfig = JsonConvert.DeserializeObject<ConfigInfo>(info.Config);
            Operate.Init();
            WebIoc.Register(b => b.Register(c=>new Loger(PluginIoc.Container.Resolve<ILogOperate>())).As<ILoger>().SingleInstance(), assemblyGuid, info.Status == PluginStatus.Usable);
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


        public PluginInfo Info
        {
            get { if (null == pluginInfo) throw new Exception("插件未进行初始化"); return pluginInfo; }
        }
    }
}
