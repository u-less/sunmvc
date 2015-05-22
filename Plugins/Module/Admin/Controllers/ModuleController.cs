using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Framework.Login;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.BaseOperate.DbContext;


namespace Plugin.Admin.Controllers
{
    public class ModuleController : AdminBaseController
    {
        public IModuleOp ModuleOp;
        public ModuleController(IModuleOp op)
        {
            ModuleOp = op;
        }
        //模块管理
        [AdminModule]
        public ActionResult Index()
        {
            ViewData["limitsStr"] = LimitsStr;
            return View();
        }

        // 获取模块管理模块Gird列表
        [AdminModule]
        public string ModuleGridJson()
        {
            if (Limits.Contains(1))
            {
                return ModuleOp.GetModuleGridJson();
            }
            else
                return string.Empty;
        }
        // 获取模块管理模块comboxtree组件的json数据(不受权限控制)
        public string ModuleComboTreeJson()
        {
            return ModuleOp.GetModuleComboTreeJson();
        }
        // 获取模块comboxtree组件的json数据(不受权限控制不含根模块)
        public string ModuleNoRootComboTreeJson()
        {
            return ModuleOp.GetModuleNoRootComboTreeJson();
        }
        //插入或者修改模块
        [AdminModule]
        public JsonResult EditOrUpdateModule(Module module)
        {
            if (ModuleOp.KeyExits(module.ModuleKey, module.ModuleId))
                return Json("模块标识(key)已经存在");
            if (module.ModuleId != 0 && Limits.Contains(2))
            {
                return Json(ModuleOp.Update(module) > 0);
            }
            else
            {
                if (Limits.Contains(3))
                {
                    module.IsUsable = true;
                    return Json(Convert.ToInt32(ModuleOp.Add(module)) != 0);
                }
                else
                    return Json(false);
            }
        }
        //删除模块
        [AdminModule]
        public bool DeleteModule(Int32 ModuleId)
        {
            if (ModuleId != 0 && Limits.Contains(4))
            {
                ModuleOp.Delete(ModuleId);
                return true;
            }
            else
                return false;
        }
        //获取模块类别combo的模块类别json数据(不受权限影响)
        public JsonResult ModuleTypeComboJson()
        {
            JsonResult JR = new JsonResult();

            JR.Data = ModuleOp.GetModuelTypeCombo();
            return JR;
        }
        /// <summary>
        /// 获取模块key列表
        /// </summary>
        /// <returns></returns>
        public JsonResult AdminModuleKeyList()
        {
            return Json(AdminModuleAttribute.keyList.Select(k => new {key=k.Key,value=k.Value }));
        }
	}
}