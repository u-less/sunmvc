using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Sun.Framework.Plugin.MVC;

namespace Plugin.Admin
{
    public class RouteConfig
    {
        public static void RegisterRoutes()
        {
            RouteManage.Add(routes =>
                routes.MapRoute(
                name: "admin",
                url: "admin/{controller}/{action}/{id}",
                defaults: new { controller = "AdminHome", action = "Index", id = UrlParameter.Optional, areaname = "admin" })
                );
            RouteManage.Add(routes=>
                routes.MapRoute(
                name: "idpage",
                url: "admin/{controller}/{action}/{id}/{page}",
                defaults: new { controller = "AdminHome", action = "Login", id = UrlParameter.Optional, page = UrlParameter.Optional, areaname = "admin" })
                );
        } 
    }
}