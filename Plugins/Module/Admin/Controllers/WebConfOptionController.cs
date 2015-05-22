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
    public class WebConfOptionController  : AdminBaseController
    {
        IConfOptionOp DbOp;
        public WebConfOptionController(IConfOptionOp op)
        {
            this.DbOp = op;
        }
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;
            return View();
        }

        //新增和编辑
        public ActionResult Edit(int optionId = 0)
        {
            ConfigOptionGrid option;
            if (optionId != 0)
                option = DbOp.GetOptionGridById(optionId);
            else
                option = new ConfigOptionGrid();
            return View(option);
        }

        //获取配置内容分页列表
        public JsonResult GetConfOptionPageJson(int page = 1, int rows = 30, int groupId = 0, int configId = 0, string optionName = null)
        {
            if (Limits.Contains(1))
            {
                GridPage<ConfigOptionGrid> data = new GridPage<ConfigOptionGrid>();
                var list = DbOp.GetPageList(page, rows, groupId, configId, optionName);
                data.total = list.TotalItems;
                data.rows = list.Items;
                return Json(data);
            }
            else
                return null;
        }
        ////新增或修改数据
        public string EditOrUpdateOption(ConfigOption option)
        {
            if (option.OptionId != 0)
            {
                if (Limits.Contains(2))
                {
                    DbOp.Update(option);
                }
                else
                    return "你没有权限进行修改";
            }
            else
            {
                if (Limits.Contains(3))
                {
                    DbOp.Add(option);
                }
                else
                    return "你没有权限新增数据";
            }
            return "True";
        }

        ////删除数据
        public string DeleteOption(int optionId)
        {
            if (Limits.Contains(4))
            {
                try
                {
                    DbOp.Delete(optionId);
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