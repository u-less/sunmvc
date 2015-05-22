using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Framework.Login;
using Sun.Core.Login;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.BaseOperate.DbContext;
using Sun.BaseOperate.Config;
using Sun.Framework.Security;
using Autofac;
using Sun.Core.Ioc;


namespace Plugin.Admin.Controllers
{
    public class AdminHomeController  : Controller
    {
        IModuleOp DbOp;
        public AdminHomeController(IModuleOp op)
        {
            DbOp = op;
        }
        [AdminLogin]
        public ActionResult Index()
        {
            ViewData["menus"] = DbOp.GetRoleModuleJson(LoginFac.Admin.GetLoginInfo().RoleId);
            return View();
        }
        /// <summary>
        /// 安全退出AJAX调用
        /// </summary>
        /// <returns>操作结果</returns>
        [HttpPost]
        public string LoginOut()
        {
            try
            {
                LoginFac.Admin.RemoveLoginInfo();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "True";
        }
        //登陆主页
        public ActionResult Login()
        {
            return View();
        }
        //登陆信息提交页
        [HttpPost]
        public JsonResult Login(string account, string password, string captcha = null, bool remember = false)
        {
            JsonResult result = new JsonResult();
            var state = LoginFac.Admin.Login(account, password, captcha, remember);
            var errors = LoginFac.Admin.GetErrors();
            result.Data = "{state:" + (int)state + ",errors:" + errors + "}";
            return result;
        }
        /// <summary>
        /// 修改密码AJAX调用
        /// </summary>
        /// <param name="password">新密码</param>
        /// <param name="repassword">确认新密吗</param>
        /// <returns>操作结果</returns>
        [HttpPost]
        public string UpdatePassword(string password, string repassword)
        {
            //服务端验证
            if (String.IsNullOrEmpty(password))
            {
                return "新密码不允许为空";
            }

            if (String.IsNullOrEmpty(repassword))
            {
                return "请再一次输入新密码";
            }

            if (password != repassword)
            {
                return "两次输入密码不一致";
            }
            WebIoc.Container.Resolve<IUserInfoOp>().UpdatePassword(password,LoginFac.Admin.GetLoginInfo().UserId);
            LoginFac.Admin.RemoveLoginInfo();
            return "True";
        }
    }
}