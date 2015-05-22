using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;
using Sun.Core.Plugin;
using Sun.Core.Ioc;
using Autofac;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Optimization;
using System.IO;
using Autofac.Integration.Mvc;
using Sun.Framework.Plugin.MVC;

namespace Sun.Framework.Plugin
{
    public class Manage:IManage
    {
        ILoader fileParser = Loader.Instance;
        static Manage _instance = new Manage();
		/// <summary>
		/// 启用单例模式
		/// </summary>
        public static Manage Instance
        {
            get
            {
                return _instance;
            }
        }
        /// <summary>
        /// 插件字典。
        /// </summary>
        private static ConcurrentDictionary<string, PluginInfo> plugins = new ConcurrentDictionary<string, PluginInfo>();
        /// <summary>
        /// 初始化。
        /// </summary>
        public void Initialize()
        {
            //注册插件控制器工厂。
            ControllerBuilder.Current.SetControllerFactory(new PluginControllerFactory());
            //注册插件模板引擎。
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new PluginRazorViewEngine());

            var plugins=fileParser.Load();
            List<Type> controllerTypes=new List<Type>();
            foreach (var plugin in plugins)
            {
                //初始化插件。
                if (plugin.Status == PluginStatus.Usable)
                    InitPlugin(plugin);
                if (plugin.Status == PluginStatus.Usable && plugin.Type == PluginType.Module)
                {
                    var baseType = typeof(AreaRegistration);
                    foreach (var t in plugin.Types)
                    {
                        if (t.BaseType.Name == baseType.Name)
                        {
                            AreaRegistration registration = (AreaRegistration)Activator.CreateInstance(t);
                            registration.CreateContextAndRegister(RouteTable.Routes, null);
                        }
                        else if (t != null &&
                                  t.IsPublic &&
                                  t.Name.EndsWith("Controller", StringComparison.OrdinalIgnoreCase) &&
                                  !t.IsAbstract &&
                                  typeof(IController).IsAssignableFrom(t))
                        {
                            controllerTypes.Add(t);
                        }
                    }
                }
            }
			RouteTable.Routes.MapMvcAttributeRoutesForTypes(controllerTypes);
            RouteManage.RegisterAllRoutes(RouteTable.Routes);
            BundleManage.RegisterAllBundles(BundleTable.Bundles);
            WebIoc.Register(b => b.RegisterTypeFromDirectory(null,AppDomain.CurrentDomain.SetupInformation.PrivateBinPath));
            WebIoc.Register(b => b.RegisterTypeFromDirectory(null, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\Plugins")));
            WebIoc.Register(b => b.RegisterControllersFromDirectory(null, Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"App_Data\Plugins")));
            WebIoc.Register(b => b.RegisterControllers(AppDomain.CurrentDomain.GetAssemblies()));
            WebIoc.Instance.OnBuilded+=c=> DependencyResolver.SetResolver(new AutofacDependencyResolver(c));
            WebIoc.Instance.Build();
        }
		/// <summary>
		/// 初始化插件。
		/// </summary>
		/// <param name="pluginDescriptor">插件描述</param>
        public void InitPlugin(PluginInfo pluginInfo)
        {
            //使用插件名称做为字典 KEY。
            string key = pluginInfo.Plugin.Name;
            //不存在时才进行初始化。
            if (!plugins.ContainsKey(key))
            {
                //初始化。
                try
                {
                    pluginInfo.Plugin.Initialize(pluginInfo);
                }
                catch (Exception e)
                {
                    pluginInfo.Status = PluginStatus.Error;
                    FileOperate.Instance.SavePluginInfo(pluginInfo);
                }
                //增加到字典。
                plugins.TryAdd(key, pluginInfo);
            }
        }


        /// <summary>
        /// 获得当前系统所有插件描述。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PluginInfo> GetPlugins()
        {
            return plugins.Values;
        }

        public PluginInfo GetPluginByName(string name)
        {
            PluginInfo plugin;
            if (plugins.TryGetValue(name, out plugin))
            {
                return plugin;
            }
            else
            {
                return null;
            }
        }

        public PluginInfo GetPluginById(string guid)
        {
            return GetPlugins().SingleOrDefault(p => p.Guid == guid);
        }

		public void UnloadPlugin(string guid)
		{
            var pluginInfo = GetPluginById(guid);
			if (pluginInfo.Status != PluginStatus.Stop)
			{
				StopPlugin(guid);
			}
            pluginInfo.Status = PluginStatus.NeedUnload;
            var fileOp = FileOperate.Instance;
            fileOp.SavePluginInfo(pluginInfo);
            //plugins.TryRemove(pluginInfo.Plugin.Name, out pluginInfo);
        }


		public void StartPlugin(string guid)
		{
            var pluginInfo = GetPluginById(guid);
			try
			{
				if (pluginInfo.Status != PluginStatus.Usable)
				{
					pluginInfo.Status = PluginStatus.Usable;
					pluginInfo.Plugin.Start();
					FileOperate.Instance.SavePluginInfo(pluginInfo);
				}
			}
			catch {
				pluginInfo.Status = PluginStatus.Error;
				FileOperate.Instance.SavePluginInfo(pluginInfo);
			}
		}

		public void StopPlugin(string guid)
		{
            var pluginInfo = GetPluginById(guid);
			if (pluginInfo.Status != PluginStatus.Stop)
			{
				pluginInfo.Status = PluginStatus.Stop;
				pluginInfo.Plugin.Stop();
				FileOperate.Instance.SavePluginInfo(pluginInfo);
			}
		}

        ConcurrentDictionary<string, PluginInfo> areas = new ConcurrentDictionary<string, PluginInfo>();
        public PluginInfo GetPluginByAreaName(string areaName)
        {
            PluginInfo plugin;
            if (!areas.TryGetValue(areaName, out plugin))
            {
                plugin =GetPlugins().SingleOrDefault(p=>p.Areas!=null&&p.Areas.Keys.Contains(areaName.ToLower()));
                if (null != plugin)
                    areas.TryAdd(areaName, plugin);
            }
            return plugin;
        }
    }
}
