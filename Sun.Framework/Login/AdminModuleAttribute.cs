using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections.Concurrent;
using Sun.Core.Login;
using Sun.Core.Ioc;
using Autofac;

namespace Sun.Framework.Login
{
    public class AdminModuleAttribute:AdminLoginAttribute
    {
        /// <summary>
        /// 模块key列表
        /// </summary>
        public static ConcurrentDictionary<string, string> keyList = new ConcurrentDictionary<string, string>();
        public string key;
        public string name;
        public AdminModuleAttribute(string moduleKey = null, string moduleName = null)
        {
            key = moduleKey; name = moduleName;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrEmpty(key))
            {
                key = filterContext.Controller.ToString();
                if (!keyList.ContainsKey(key))
                {
                    if (string.IsNullOrEmpty(name)) name = key;
                    keyList.TryAdd(key, name);
                }
            }
            base.OnActionExecuting(filterContext);
            if (filterContext.Result == null && !CanIntoModule(key, filterContext))
                filterContext.Result = new RedirectToRouteResult("Default", new System.Web.Routing.RouteValueDictionary(new { controller = "adminhome", action = "NoPerMission" }));
        }
        private bool CanIntoModule(string moduleKey, ActionExecutingContext filterContext)
        {
            var powerList = WebIoc.Container.Resolve<IModuleLimit>().GetLimitsByKey(moduleKey, LoginFac.Admin.GetLoginInfo().RoleId.ToString());//获取权限信息
            if (powerList.Count() > 0)
            {
                filterContext.Controller.ViewData["Limits"] = powerList;//把权限信息存储到http的上下文
                return true;
            }
            return false;
        }
    }
}
