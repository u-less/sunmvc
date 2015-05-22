using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Framework.Login;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.Model.DBExtensions;using Sun.Model.Common;
using Sun.BaseOperate.DbContext;
using Sun.Core.Login;
using Autofac;
using Sun.Core.Ioc;
using Sun.Framework.Security;

namespace Plugin.Admin.Controllers
{
    [AdminModule]
    public class SysUserInfoController  : AdminBaseController
    {
        IUserInfoOp DbOp;
        public SysUserInfoController(IUserInfoOp op)
        {
            DbOp = op;
        }
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;//当前用户在该模块拥有的权限拼接字符串，以#分割，提供给js做权限控制
            return View();
        }
        //查看用户详情
        public ActionResult Detail(int userId = 0)
        {
            UserInfo user;
            if (userId != 0)
                user = DbOp.GetModelById(userId);
            else
                user = new UserInfo();
            return View(user);
        }
        //编辑用户信息
        public ActionResult Edit(int userId=0)
        {
            UserInfo user;
            if (userId != 0)
                user = DbOp.GetModelById(userId);
            else
                user = new UserInfo();
            return View(user);
        }
        /// <summary>
        /// 获取用户分页数据
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Rows"></param>
        /// <returns></returns>
        public JsonResult UserGridJson(int page = 1, int rows = 30, string username = null, string sex = null, string states = null, string usertype = null, string regionid = null, string organid = null, string roleid = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            JsonResult result = new JsonResult();
            if (Limits.Contains(1))
            {
                GridPage<UserGrid> jr = new GridPage<UserGrid>();
                var oIds = LoginFac.Admin.GetLoginInfo().OrganId;
                if (!string.IsNullOrEmpty(organid)&&oIds.Split(',').Any(o => organid.StartsWith(o)))
                    oIds = organid;
                var data = DbOp.GetPageList(page, rows, username, sex, states, usertype, regionid, oIds, roleid, starttime, endtime);
                jr.rows = data.Items;
                jr.total = data.TotalItems;
                result.Data = jr;
            }
            return result;
        }
        /// <summary>
        /// 获取角色的列表用于combox
        /// </summary>
        /// <returns></returns>
        public JsonResult GetRoleCombox()
        {
            JsonResult result = new JsonResult();
            result.Data = WebIoc.Container.Resolve<IRoleOp>().RoleKeyValues();
            return result;
        }

        /// <summary>
        /// 获取类型的列表用于combox
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserTypeCombox()
        {
            JsonResult result = new JsonResult();
            result.Data = DbOp.GetUserTypes();
            return result;
        }

        /// <summary>
        /// 获取状态的列表用于combox
        /// </summary>
        /// <returns></returns>
        public JsonResult GetUserStatusCombox()
        {
            JsonResult result = new JsonResult();
            result.Data = DbOp.GetUserStatus();
            return result;
        }

        //插入或者修改用户
        public bool EditOrUpdateUser(UserInfo user)
        {
            user.OrganIds=Request.Form["OrganIds"];
            user.LoginId = user.LoginId.ToLower();
            user.Email = (user.Email ?? "").ToLower();
            if (user.UserId != 0 && Limits.Contains(2))
            {
                if(!String.IsNullOrEmpty(user.Password))
                {
                    user.Password =Md5Encrypt.PasswordEncode(user.Password);
                    return Context.Instance.Update(user, new string[] { "password", "phonenumber", "email", "username", "states", "regionid", "roleid", "organid", "usertype", "sex", "nickname" }) > 0;
                }
                else
                    return Context.Instance.Update(user, new string[] { "phonenumber", "email", "username", "states", "regionid", "roleid", "organid", "usertype", "sex", "nickname" }) > 0;
            }
            else
            {
                user.AddTime = DateTime.Now;
                user.LastLoginTime = DateTime.Now;
                user.Password = Md5Encrypt.PasswordEncode(user.Password);
                if (Limits.Contains(3))
                {
                    return DbOp.Add(user) != null;
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        public string DeleteUser(string userId)
        {
            if (Limits.Contains(4))
            {
                return DbOp.Delete(userId).ToString();
            }
            else
                return "你没有权限删除数据";
        }
	}
}