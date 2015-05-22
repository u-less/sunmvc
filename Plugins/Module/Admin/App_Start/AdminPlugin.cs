using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;
using System.Reflection;
using System.Web.Routing;
using System.Web.Optimization;
using Sun.Core.Plugin;
using Sun.Core.Ioc;
using Autofac;
using Sun.Framework.Plugin;
using Sun.Framework.Plugin.MVC;
using Plugin.Admin.Controllers;

namespace Plugin.Admin
{
    [IocExport(typeof(IPlugin), ContractName = "2304e992-d781-4d04-8504-2d4e45c4e58f")]
    public class AdminPlugin : IPlugin
    {
        public string Name
        {
            get
            {
                return "Admin";
            }
        }
        public PluginInfo Info
        {
            get { if (null == pluginInfo) throw new Exception("插件未进行初始化"); return pluginInfo; }
        }
		internal static PluginInfo pluginInfo;
        public void Initialize(PluginInfo info)
        {
            if (null == pluginInfo)
			{
				pluginInfo = info;
				RouteConfig.RegisterRoutes();//注册路由
				BundleConfig.RegisterBundles();//注册css与javascript压缩
                WebIoc.Register(c => c.RegisterInstance<RouteValueDictionary>(new System.Web.Routing.RouteValueDictionary(new { controller = "adminhome", action = "login",area="admin" })).Named<RouteValueDictionary>("admin"));
			}
        }

        public void Stop()
        {
            IFileOperate fileOp = WebIoc.Container.Resolve<IFileOperate>();
            Info.Status = PluginStatus.Stop;
            fileOp.SavePluginInfo(pluginInfo);
        }

        public void Start()
        {
            IFileOperate fileOp = WebIoc.Container.Resolve<IFileOperate>();
            Info.Status = PluginStatus.Usable;
            fileOp.SavePluginInfo(pluginInfo);
        }
    }
}