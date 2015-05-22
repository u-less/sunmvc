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
using Sun.BaseOperate.Config;

namespace Plugin.Admin.Controllers
{
    [AdminModule]
    public class WebConfigSetController  : AdminBaseController
    {
        IConfigOp DbOp;
        IConfOptionOp DbOptionOp;
        public WebConfigSetController(IConfigOp op,IConfOptionOp optionOp)
        {
            this.DbOp = op; this.DbOptionOp = optionOp;
        }
        //网站配置内容设置
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;
            return View();
        }
        //获取配置分页列表
        public JsonResult GetConfSetData()
        {
            if (Limits.Contains(1))
            {
                var confs = DbOp.GetConfList();
                var options = DbOptionOp.GetOptionList();
                foreach (var c in confs)
                {
                    if (c.CType == 0 || c.CType == 1)
                    {
                        c.Options = options.Where(o => o.ConfigId == c.ConfigId).ToList();
                    }
                }
                GridPage<ConfigSet> data = new GridPage<ConfigSet>();
                data.total = confs.Count();
                data.rows = confs;
                return Json(data);
            }
            else
                return null;
        }
        //通过Key设置值
        [ValidateInput(false)]
        public JsonResult SetValueById(int confId, string value,string key)
        {
            var result = DbOp.SetValueByKey(confId, value);
            if (result)
                result = DbOp.ClearConfigCache(key);
            return Json(result);
        }
	}
}