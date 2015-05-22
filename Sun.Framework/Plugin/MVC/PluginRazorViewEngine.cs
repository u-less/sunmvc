using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Web.Mvc;
using System.Reflection;
using System.Web.Routing;
using System.Web.WebPages.Razor;
using Sun.Core.Plugin;
using Sun.Core.Ioc;
using Autofac;

namespace Sun.Framework.Plugin.MVC
{
    [IocExport(typeof(IViewEngine))]
    public class PluginRazorViewEngine : RazorViewEngine
    {
        IManage manage = new Manage();

        private string[] _areaViewLocationFormats = new string[]
			{
                "~/Plugins/Module/{2}/Views/{1}/{0}.cshtml",
                "~/Plugins/Module/{2}/Views/{1}/{0}.vbhtml",
                "~/Plugins/Module/{2}/Views/Shared/{0}.cshtml",
                "~/Plugins/Module/{2}/Views/Shared/{0}.vbhtml",
				"~/Areas/{2}/Views/{1}/{0}.cshtml",
				"~/Areas/{2}/Views/{1}/{0}.vbhtml",
				"~/Areas/{2}/Views/Shared/{0}.cshtml",
				"~/Areas/{2}/Views/Shared/{0}.vbhtml"
			};
        private string[] _viewLocationFormats = new string[]
			{
				"~/Views/{1}/{0}.cshtml",
				"~/Views/{1}/{0}.vbhtml",
				"~/Views/Shared/{0}.cshtml",
				"~/Views/Shared/{0}.vbhtml"
			};

        /// <summary>
        /// 空构造函数
        /// </summary>
        public PluginRazorViewEngine()
            : this(null)
        {
        }
        /// <summary>
        /// 初始化视图引擎
        /// </summary>
        /// <param name="viewPageActivator"></param>
        public PluginRazorViewEngine(IViewPageActivator viewPageActivator)
            : base(viewPageActivator)
        {
            base.AreaViewLocationFormats = _areaViewLocationFormats;
            base.AreaMasterLocationFormats = _areaViewLocationFormats;
            base.AreaPartialViewLocationFormats = _areaViewLocationFormats;

            base.ViewLocationFormats = _viewLocationFormats;
            base.MasterLocationFormats = _viewLocationFormats;
            base.PartialViewLocationFormats = _viewLocationFormats;

            base.FileExtensions = new string[]
			{
				"cshtml",
				"vbhtml"
			};
        }

        /// <summary>
        /// 搜索部分视图页。
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="partialViewName"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            UpdateRouteData(controllerContext);
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }
        /// <summary>
        /// 查找视图
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="viewName"></param>
        /// <param name="masterName"></param>
        /// <param name="useCache"></param>
        /// <returns></returns>
        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            UpdateRouteData(controllerContext);
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        private void UpdateRouteData( ControllerContext controllerContext)
        {
            object area;
            if (controllerContext.RouteData.DataTokens.TryGetValue("area", out area))
            {
                var directoryName = CodeGeneration(area.ToString());
                if (directoryName != null)
                {
                    controllerContext.RouteData.DataTokens["area"] = directoryName;
                }
            }
        }
        ConcurrentDictionary<string, string> directorylist = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 给运行时编译的页面加了引用程序集。
        /// </summary>
        /// <param name="pluginName"></param>
        private string CodeGeneration(string areaname)
        {
            string directoryName;
            if (!directorylist.TryGetValue(areaname.ToLower(), out directoryName))
            {
                var plugin = manage.GetPluginByAreaName(areaname.ToLower());
                if (plugin != null)
                {
                    plugin.Areas.TryGetValue(areaname.ToLower(), out directoryName);
                    if(!string.IsNullOrEmpty(directoryName))
                        directoryName =plugin.DirectoryName+directoryName;
                    else
                        directoryName = plugin.DirectoryName;
                    directorylist.TryAdd(areaname.ToLower(), directoryName);
                    RazorBuildProvider.CodeGenerationStarted += (object sender, EventArgs e) =>
                    {
                        RazorBuildProvider provider = (RazorBuildProvider)sender;
                        var pluginInfo = manage.GetPluginByAreaName(areaname);
                        if (pluginInfo != null)
                        {
                            provider.AssemblyBuilder.AddAssemblyReference(pluginInfo.PluginAssembly);
                            var refrences = pluginInfo.PluginAssembly.GetReferencedAssemblies();
                            if (refrences.Length > 0)
                            {
                                foreach (var assem in refrences)
                                {
                                    provider.AssemblyBuilder.AddAssemblyReference(Assembly.Load(assem));
                                }
                            }
                        }
                    };
                }
            }
            return directoryName;
        }

    }
}
