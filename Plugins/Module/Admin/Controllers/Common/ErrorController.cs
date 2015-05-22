using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Sun.BaseOperate.Config;

namespace Plugin.Admin.Controllers.Common
{
    public class ErrorController  : Controller
    {
        //403错误提示页
        public ActionResult E403()
        {
            return View();
        }
        //404错误
        public ActionResult E404()
        {
            return View();
        }
	}
}