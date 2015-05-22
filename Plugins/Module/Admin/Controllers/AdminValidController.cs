using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.BaseOperate.DbContext;
using System.ComponentModel;
using Autofac;
using Sun.Core.Ioc;

namespace Plugin.Admin.Controllers
{
    /// <summary>
    /// 远程表单验证控制器
    /// </summary>
    public class AdminValidController  : AdminBaseController
    {
        //验证账号不存在
        public JsonResult AccountNotExits(string loginId)
        {
            var Data = !WebIoc.Container.Resolve<IUserInfoOp>().AccountExist(loginId);
            return Json(Data);
        }
        //验证网站配置key是否存在
        public JsonResult ConfigKeyExits(string ckey)
        {
            var data = !WebIoc.Container.Resolve<IConfigOp>().KeyExits(ckey);
            return Json(data);
        }
    }
}