using System.Web.Mvc;

namespace Plugin.Admin.Areas.One
{
    public class OneAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "One";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "One_default",
                "One/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}