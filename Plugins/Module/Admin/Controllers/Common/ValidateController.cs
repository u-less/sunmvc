using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Model.DB;
using Sun.BaseOperate.Interface;

namespace Plugin.Admin.Controllers.Web
{
    public class ValidateController  : Controller
    {
        IUserInfoOp UserOp;
        public ValidateController(IUserInfoOp userOp)
        {
            this.UserOp = userOp;
        }
        //验证管理员账号存在
        [HttpPost]
        public JsonResult AccountExits(string account)
        {
            var data = UserOp.AccountExist(account);
            return Json(data);
        }
        //验证邮箱是否已被绑定使用
        [HttpPost]
        public JsonResult EmailExits(string email)
        {
            var data = UserOp.EmailExist(email);
            return Json(data);
        }
        //验证手机号码是否已被绑定使用
        [HttpPost]
        public JsonResult PhoneNumberExits(string phone)
        {
            var data = UserOp.PhoneNumberExist(phone);
            return Json(data);
        }
	}
}