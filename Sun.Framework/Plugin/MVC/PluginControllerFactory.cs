using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web.Mvc;
using Sun.Core.Plugin;
using Sun.Core.Ioc;
using Autofac;
using Sun.Framework.Plugin;

namespace Sun.Framework.Plugin.MVC
{
    /// <summary>
    /// 插件控制器工厂。
    /// </summary>
    public class PluginControllerFactory : DefaultControllerFactory
    {
        IManage manage =Manage.Instance;
        static ConcurrentDictionary<string, Type> pluginControllers = new ConcurrentDictionary<string, Type>();
        /// <summary>
        /// 根据控制器名称及请求信息获得控制器类型。
        /// </summary>
        /// <param name="requestContext">请求信息</param>
        /// <param name="controllerName">控制器名称。</param>
        /// <returns>控制器类型。</returns>
        protected override Type GetControllerType(RequestContext requestContext, string controllerName)
        {
            object area;
            string areaName =string.Empty;
            Type controllerType = null;
            if (requestContext.RouteData.DataTokens.TryGetValue("area", out area))
            {
                areaName = area.ToString();
            }
            else
            {
                if (requestContext.RouteData.Values.ContainsKey("areaname"))
                {
                    areaName = requestContext.RouteData.GetRequiredString("areaname");
                    requestContext.RouteData.DataTokens["area"] = areaName;
                }
            }
            if(!string.IsNullOrEmpty(areaName))
            controllerType = GetControllerType(areaName, controllerName);
            if(null==controllerType)
                controllerType = base.GetControllerType(requestContext, controllerName);
            return controllerType;
        }
        /// <summary>
        /// 根据控制器名称获得控制器类型。
        /// </summary>
        /// <param name="controllerName">控制器名称。</param>
        /// <returns>控制器类型。</returns>
        private Type GetControllerType(string areaName, string controllerName)
        {
            Type type;
            string key=areaName + "/" + controllerName;
            if (pluginControllers.TryGetValue(key.ToLower(), out type))
                return type;
            else
            {
                var controlName = controllerName + "Controller";
                var plugin = manage.GetPluginByAreaName(areaName);
                if (plugin != null)
                {
                    type = plugin.Types.FirstOrDefault(p => p.Name.ToLower() == controlName.ToLower());
                    pluginControllers.TryAdd(key, type);
                    return type;
                }
                return null;
            }
        }
    }
}
