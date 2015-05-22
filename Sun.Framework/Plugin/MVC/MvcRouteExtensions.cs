using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Routing;
using System.Web.Routing;

namespace Sun.Framework.Plugin.MVC
{
    public static class MvcRouteExtensions
    {
        /// <summary>
        /// 自动映射Ctroller路由标签
        /// </summary>
        /// <param name="routes">路由集合</param>
        /// <param name="types">控制器type列表</param>
        public static void MapMvcAttributeRoutesForTypes(this RouteCollection routes, List<Type> types)
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies().SingleOrDefault(a => a.GetName().Name == "System.Web.Mvc");
            var mapMvcAttributeRoutesMethod = assembly.GetType("System.Web.Mvc.Routing.AttributeRoutingMapper")
                .GetMethod("MapAttributeRoutes", new Type[] { typeof(RouteCollection), typeof(IEnumerable<Type>) });
            mapMvcAttributeRoutesMethod.Invoke(null, new object[] { routes, types });
        }
        /// <summary>
        /// 映射Area路由数据
        /// </summary>
        /// <param name="area"></param>
        /// <param name="routes"></param>
        /// <param name="state"></param>
        public static void CreateContextAndRegister(this AreaRegistration area, RouteCollection routes, object state)
        {
            AreaRegistrationContext context = new AreaRegistrationContext(area.AreaName, routes, state);
            string thisNamespace = area.GetType().Namespace;
            if (thisNamespace != null)
            {
                context.Namespaces.Add(thisNamespace + ".*");
            }
            area.RegisterArea(context);
        }
    }
}
