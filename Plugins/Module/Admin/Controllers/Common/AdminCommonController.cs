using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Framework.Login;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.BaseOperate.DbContext;
using Sun.Model.DBExtensions;
using Sun.Model.Common;
using Sun.Core.Login;

namespace Plugin.Admin.Controllers.Common
{
    public class AdminCommonController  : AdminBaseController
    {
        IOrganOp DbOp;
        public AdminCommonController(IOrganOp op)
        {
            this.DbOp = op;
        }
        /// <summary>
        /// 获取机构分页数据
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Rows"></param>
        /// <returns></returns>
        [AdminLogin]
        public JsonResult OrganComboGridJson(int page = 1, int rows = 30, string typeId = null, string level = null, string organName = null)
        {
            GridPage<OrganGrid> jr = new GridPage<OrganGrid>();
            var data = DbOp.GetPageList(page, rows, typeId, level, organName,LoginFac.Admin.GetLoginInfo().OrganId, true); 
            jr.rows = data.Items;
            jr.total = data.TotalItems;
            return Json(jr);
        }
    }
}