using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Sun.Framework.Plugin.MVC
{
    public class RouteManage
    {
        static event Action<RouteCollection> RouteHandlers;
        public static void Add(Action<RouteCollection> routeHandler)
        {
            RouteHandlers += routeHandler;
        }
        public static void RegisterAllRoutes(RouteCollection routes)
        {
            if (routes != null&&RouteHandlers!=null)
                RouteHandlers(routes);
            RouteHandlers = null;//释放事件
        }
    }
}
