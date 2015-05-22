using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sun.Core.Login;
using Sun.Framework.Model;
using Sun.Core.Ioc;
using Autofac;

namespace Sun.Framework.Login
{
    public class AdminModuleDKAttribute : AdminLoginAttribute
    {
        //模块Id
        private string ModuleKey { get; set; }
        //moduleKey参数名
        private string Para { get; set; }
        private string ParaKey { get; set; }
        /// <summary>
        /// 构造函数(dynamic key)
        /// </summary>
        /// <param name="paraName">ModuleKey参数名</param>
        /// <param name="valParaName">ModuleValue的参数名</param>
        public AdminModuleDKAttribute(string paraName, string valParaName)
        {
            Para = paraName;
            ParaKey = valParaName;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            ModuleKey = System.Web.HttpContext.Current.Request[Para];
            filterContext.Controller.ViewData[ParaKey] = WebIoc.Container.Resolve<IModuleLimit>().GetModuleValue(ModuleKey);
            filterContext.Controller.ViewData[Para] = ModuleKey;
            if (filterContext.Result == null && !CanIntoModule(ModuleKey,filterContext))
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
