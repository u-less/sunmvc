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
using Sun.BaseOperate.Config;
using Sun.Core;

namespace Plugin.Admin.Controllers
{
    [AdminModule]
    public class WebConfigController  : AdminBaseController
    {
        IConfigOp DbOp;
        public WebConfigController(IConfigOp op)
        {
            this.DbOp = op;
        }
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;
            return View();
        }
        //新增和编辑
        public ActionResult Edit(int configId = 0)
        {
            ConfigInfo conf;
            if (configId != 0)
                conf = DbOp.GetModelById(configId);
            else
                conf = new ConfigInfo();
            return View(conf);
        }
        //获取配置分页列表
        public JsonResult GetConfPageJson(int page = 1, int rows = 30,int groupId = 0,string opType=null,string confName=null)
        {
            if (Limits.Contains(1))
            {
                GridPage<ConfigGrid> data = new GridPage<ConfigGrid>();
                int type = string.IsNullOrEmpty(opType) ? -1 : int.Parse(opType);
                var list = DbOp.GetPageList(page, rows, groupId, type, confName);
                data.total = list.TotalItems;
                data.rows = list.Items;
                return Json(data);
            }
            else
                return null;
        }
        //新增或修改数据
        public string EditOrUpdateConf(ConfigInfo conf)
        {
            if (conf.ConfigId != 0)
            {
                if (Limits.Contains(2))
                {
                    DbOp.Update(conf);
                }
                else
                    return "你没有权限进行修改";
            }
            else
            {
                if (Limits.Contains(3))
                {
                    DbOp.Add(conf);
                }
                else
                    return "你没有权限新增数据";
            }
            return "True";
        }
        //删除数据
        public string DeleteConf(int configId)
        {
            if (Limits.Contains(4))
            {
                try
                {
                    DbOp.Delete(configId);
                }
                catch (Exception e)
                {
                    return e.Message;
                }
            }
            return "True";
        }
        /// <summary>
        /// 获取配置组数据用于combox
        /// </summary>
        /// <returns></returns>
        public JsonResult GetGroupCombox()
        {
            var data = DbOp.GetGroupList();
            return Json(data);
        }
        /// <summary>
        /// 获取配置类型用于combox
        /// </summary>
        /// <returns>json数据</returns>
        public JsonResult GetOpTypeCombox()
        {
            var edata = EnumExtend<ConfElementType>.GetEnumKeyName();
            IEnumerable<KeyValue> data = edata.Select(o => new KeyValue { key=o.Key.ToString(),value=o.Value});
            return Json(data);
        }
         
        /// <summary>
        /// 获取配置项数据用于combox
        /// </summary>
        /// <returns></returns>
        public JsonResult GetConfComboxByGroupId(int groupid=0)
        {
            var data = DbOp.GetListByGroupId(groupid);
            return Json(data);
        }
	}
}