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

namespace Plugin.Admin.Controllers
{
    [AdminModule]
    public class SysLimitController  : AdminBaseController
    {
        ILimitOp DbOp;
        public SysLimitController(ILimitOp op)
        {
            this.DbOp = op;
        }
        //
        // GET: /SysLimit/
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;
            return View();
        }
        //获取权限分页列表
        public JsonResult GetLimitPageJson(int moduleId, int page = 1, int rows = 30)
        {
            JsonResult jresult = new JsonResult();
            if (Limits.Contains(1))
            {;
                var data = DbOp.GetPageList(moduleId, page, rows);
                GridPage<LimitModuleName> result = new GridPage<LimitModuleName>();
                result.total = data.TotalItems;
                result.rows = data.Items;
                jresult.Data = result;
            }
            return jresult;
        }
        //新增或修改权限
        public JsonResult EditOrUpdateLimit(Limit limitInfo)
        {
            if (DbOp.KeyExits(limitInfo.Code, limitInfo.ModuleId, limitInfo.LimitId))
                return Json("权限编码已经存在,请重新设置");
            if (limitInfo.LimitId != 0)
            {
                if (Limits.Contains(2))
                {
                    DbOp.Update(limitInfo);
                }
                else
                    return Json("你没有权限进行修改");
            }
            else
            {
                if (Limits.Contains(1))
                {
                    DbOp.Add(limitInfo);
                }
                else
                    return Json("你没有权限新增数据");
            }
            return Json(true);
        }
        //删除权限
        public string DeleteLimit(int limitId)
        {
            if (Limits.Contains(4))
            {
                try
                {
                    DbOp.Delete(limitId);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return "True";
        }
	}
}