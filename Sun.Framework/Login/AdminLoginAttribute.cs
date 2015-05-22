using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sun.Core.Login;
using Sun.Core.Ioc;
using Autofac;


namespace Sun.Framework.Login
{
    public class AdminLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (LoginFac.Admin.GetLoginInfo().UserId == 0)
            {
                filterContext.Result = new RedirectToRouteResult("Default",WebIoc.Container.ResolveNamed<System.Web.Routing.RouteValueDictionary>("admin"));
            }
        }
    }
}
