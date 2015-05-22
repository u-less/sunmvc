using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Framework.Login;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.Model.DBExtensions;
using Sun.Model.Common;
using Sun.BaseOperate.DbContext;
using Sun.Core.Login;
using Sun.BaseOperate.Config;
using Sun.Core.Ioc;
using Autofac;

namespace Plugin.Admin.Controllers
{
    [AdminModule]
    public class SysRoleController  : AdminBaseController
    {
        IRoleOp DbOp;
        public SysRoleController(IRoleOp op)
        {
            this.DbOp = op;
        }
        //角色管理
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;
            return View();
        }
        public ActionResult RoleLimtsSet(int roleId)
        {
            ViewData["limitsStr"] = LimitsStr;
            if (!Limits.Contains(5))
                return new RedirectToRouteResult("Default", new System.Web.Routing.RouteValueDictionary(new { controller = "Admin", action = "NoLimit" }));
            return View(roleId);
        }
        //角色信息获取
        public JsonResult RolePageJson(int Page = 1, int rows = 30)
        {
            JsonResult JR = new JsonResult();
            if (Limits.Contains(1))
            {
                var data = DbOp.GetPageList(Page, rows);
                GridPage<RoleGrid> result = new GridPage<RoleGrid>();
                result.total = data.TotalItems;
                result.rows=data.Items;
                JR.Data = result;
            }
            return JR;
        }
        //新增或修改角色
        public string EditOrUpdateRole(Role role)
        {
            var loginInfo =LoginFac.Admin.GetLoginInfo();
            role.AddTime = DateTime.Now;
            role.AdminId = loginInfo.UserId;
            if (role.RoleId != 0)
            {
                if (Limits.Contains(2)&&((role.OrganId==0&&loginInfo.RoleId==CustomConfig.SuperRoleId)||role.OrganId.ToString().StartsWith(loginInfo.OrganId.ToString())))
                {
                    DbOp.Update(role);
                    return "True";
                }
                else
                    return "你没有权限进行修改";
            }
            else
            {
                if (Limits.Contains(3) && ((role.OrganId == 0 && loginInfo.RoleId == CustomConfig.SuperRoleId) || role.OrganId.ToString().StartsWith(loginInfo.OrganId.ToString())))
                {
                    DbOp.Add(role);
                    return "True";
                }
                else
                    return "你没有权限新增数据";
            }
        }
        //删除角色
        public string DeleteRole(int roleId)
        {
            if (Limits.Contains(4))
            {
                if (DbOp.Delete(roleId)>0)
                    return "True";
                else
                    return "删除失败";
            }
            else
                return "你没有权限删除数据";
        }
        //权限分配
        public string GetLimitAboutRoleJson(int roleId)
        {
            if (Limits.Contains(5))
            {
                var limitOp = WebIoc.Container.Resolve<IRoleLimitOp>();
                var myRoleId=LoginFac.Admin.GetLoginInfo().RoleId;
                return limitOp.GetLimitAboutRoleJson(roleId, myRoleId == CustomConfig.SuperRoleId ? 0 : myRoleId);
            }
            else
                return "你没有权限获取数据";
        }
        //保存权限分配结果
        public string SaveRoleLimits(int roleId, string limitStr, string moduleStr)
        {
            if (Limits.Contains(5))
            {
                try
                {
                    string[] limitIds = limitStr.Split('|');
                    string[] ModuleIds = moduleStr.Split('|');
                    var limitOp = WebIoc.Container.Resolve<IRoleLimitOp>();
                    limitOp.SetRoleModule(roleId, ModuleIds);
                    limitOp.SetRoleLimit(roleId, limitIds);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                return "True";
            }
            else
                return "你没有权限分配权限";
        }
	}
}