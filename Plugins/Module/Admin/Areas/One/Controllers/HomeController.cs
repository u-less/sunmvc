using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Plugin.Admin.Areas.One.Controllers
{
    public class HomeController : Controller
    {
        // GET: One/Home
        public ActionResult Index()
        {
            return View();
        }
    }
}