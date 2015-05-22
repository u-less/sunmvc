using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Plugin.Admin.Controllers
{
    public class AdminBaseController  : Controller
    {
        /// <summary>
        /// 获取当前登录用户的角色在该模块的权限列表
        /// </summary>
        protected List<int> Limits
        {
            get
            {
                return ControllerContext.Controller.ViewData["Limits"] as List<int>;
            }
        }
        /// <summary>
        /// 获取当前登录用户的角色在该模块的权限字符串
        /// </summary>
        protected string LimitsStr
        {
            get
            {
                if (Limits == null) return "";
                return string.Join(",", Limits.Select(o => o.ToString()));
            }
        }
    }
}