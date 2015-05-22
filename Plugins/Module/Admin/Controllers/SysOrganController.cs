using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Framework.Login;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.Model.DB;
using Sun.BaseOperate.DbContext;
using Sun.Model.Common;
using Sun.Model.DBExtensions;
using Sun.Core.Login;
using Sun.BaseOperate.Config;

namespace Plugin.Admin.Controllers
{
    [AdminModule]
    public class SysOrganController  : AdminBaseController
    {
        IOrganOp DbOp;
        public SysOrganController(IOrganOp op)
        {
            this.DbOp = op;
        }
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;//当前用户在该模块拥有的权限拼接字符串，以#分割，提供给js做权限控制
            return View();
        }
        //新增编辑
        public ActionResult Edit(int organId=0)
        {
            Organ organ;
            if (organId != 0)
                organ = DbOp.GetModelById(organId);
            else
                organ = new Organ();
            return View(organ);
        }
        /// <summary>
        /// 获取机构分页数据
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Rows"></param>
        /// <returns></returns>
        public JsonResult OrganGridJson(int page = 1, int rows = 30, string typeId = null, string level = null,string organName = null,string parentId=null)
        {
            JsonResult result = new JsonResult();
            if (Limits.Contains(1))
            {
                GridPage<OrganGrid> jr = new GridPage<OrganGrid>();
                var loginInfo = LoginFac.Admin.GetLoginInfo();
                string pid =loginInfo.OrganId;
                var haveme = false;
                if (!string.IsNullOrEmpty(parentId)&&loginInfo.OrganId.Split(',').Any(o=> parentId.StartsWith(o)))
                {
                    pid = parentId;
                }
                else
                {
                    haveme = loginInfo.RoleId == CustomConfig.SuperRoleId;
                }
                var data = DbOp.GetPageList(page, rows, typeId, level, organName, pid, haveme);
                jr.rows = data.Items;
                jr.total = data.TotalItems;
                result.Data = jr;
            }
            return result;
        } 

        /// <summary>
        /// 获取机构类别combo的机构类别json数据(不受权限影响)
        /// </summary>
        /// <returns></returns>
        public JsonResult OrganTypeComboJson()
        {
            JsonResult JR = new JsonResult();
            if (Limits.Contains(1))
            {
                JR.Data = DbOp.GetTypes();
            }
            return JR;
        }

        /// <summary>
        /// 获取机构等级combo的机构等级json数据(不受权限影响)
        /// </summary>
        /// <returns></returns>
        public JsonResult OrganLevelComboJson()
        {
            JsonResult JR = new JsonResult();
            if (Limits.Contains(1))
            {
                JR.Data = DbOp.GetLevels();
            }
            return JR;
        }

        //新增或者修改机构
        public string EditOrUpdateOrgan(Organ organ)
        {
            if (organ.ParentId == 0 && LoginFac.Admin.GetLoginInfo().RoleId != CustomConfig.SuperRoleId)
                return "上级机构不能为空";
            if (organ.OrganId != 0 && Limits.Contains(2))
            {
                if (DbOp.Update(organ) > 0)
                    return "True";
                else
                    return "修改失败";
            }
            else
            {
                if (Limits.Contains(3))
                {
                    var id = DbOp.CreateOrganId(organ.ParentId.ToString());
                    if (string.IsNullOrEmpty(id))
                        return "所选择的上级机构不允许添加子机构";
                    else
                    {
                        organ.OrganId = Convert.ToInt32(id);
                        if (Convert.ToInt32(DbOp.Add(organ)) != 0)
                            return "True";
                        else
                            return "新增失败";
                    }
                }
                else
                    return "你没有权限新增机构";
            }
        }

        /// <summary>
        /// 删除机构
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        public string DeleteOrgan(string organId)
        {
            if (Limits.Contains(4))
            {
                try
                {
                    if (DbOp.IsParent(organId) != true)
                    {
                        DbOp.Delete(organId);
                    }
                    else
                        return "请先删除该节点的子节点";
                }
                catch (Exception e)
                {
                    return e.Message;
                }
                return "True";
            }
            else
                return "你没有权限删除数据";
        }
    }
}
