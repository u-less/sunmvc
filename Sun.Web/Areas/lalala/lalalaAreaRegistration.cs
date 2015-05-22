using System.Web.Mvc;

namespace Sun.Web.Areas.lalala
{
    public class lalalaAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "lalala";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "lalala_default",
                "lalala/{controller}/{action}/{id}",
                new {action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}