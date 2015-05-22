using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Caching;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.Model.DBExtensions;
using Sun.Model.Common;
using Sun.BaseOperate.DbContext;
using Sun.Core.Ioc;

namespace Sun.BaseOperate.DB
{
    [IocExport(typeof(IModuleOp),true)]
    public class ModuleOp :IModuleOp
    {
        public IModelCacheFac<Module> CacheOp
        {
            get;
            set;
        }
        IDictOp DictOp;
        public ModuleOp(IModelCacheFac<Module> cacheOp,IDictOp dictOp)
        {
            CacheOp = cacheOp;
            DictOp = dictOp;
        }
        public object Add(Module entity)
        {
            return Context.Instance.Insert(entity);
        }

        public int Delete(object id)
        {
            var DataContext = Context.Instance;
            var count = 0;
            DataContext.BeginTransaction();
            try
            {
                count += DataContext.Delete<SysRoleLimit>("where ModuleId=@0", id);
                count += DataContext.Delete<Limit>("where ModuleId=@0", id);
                count += DataContext.Delete<Module>("where ModuleId=@0", id);
                DataContext.CompleteTransaction();
            }
            catch
            {
                DataContext.AbortTransaction();
            }
            return count;
        }

        public Module GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<Module>(id);
        }
        public int Update(Module entity)
        {
            return Context.Instance.Update(entity);
        }

        public int Update(Module entity, string[] columns)
        {
            return Context.Instance.Update(entity, columns);
        }
        /// <summary>
        /// 根据模块的KEY获取模块
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Module GetModuleByKey(string key)
        {
            return Context.Instance.SingleOrDefault<Module>("select * from Sys_Module where ModuleKey=@0", key);
        }
        /// <summary>
        /// 判断key是否已经存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool KeyExits(string key, int id = 0)
        {
            var sql = "select moduleid from Sys_Module where ModuleKey=@0";
            if (id != 0)
                sql += " and moduleid<>@1";
            return Context.Instance.SingleOrDefault<int>(sql, key, id) > 0;
        }
        /// <summary>
        /// 修改模块
        /// </summary>
        /// <param name="module"></param>
        /// <returns></returns>
        public int UpdateModule(Module module)
        {
            return Context.Instance.Update(module);
        }
        /// <summary>
        /// 获取Module的Grid列表数据
        /// </summary>
        /// <returns></returns>
        public string GetModuleGridJson()
        {
            StringBuilder ResultJson = new StringBuilder();
            var ModuleList = Context.Instance.Query<ModuleTypeName>("select a.*,b.DictName TypeName from Sys_Module a inner join Sys_Dict b on a.TypeId=b.DictId").ToList();
            ResultJson.Append("{\"total\":" + ModuleList.Count() + ",\"rows\":[");
            ResultJson.Append(ResolveModuleGridStr(ModuleList.Where(o => o.ParentId == 0), ModuleList));
            ResultJson.Append("]}");
            return ResultJson.ToString();
        }
        /// <summary>
        /// 递归分解Module列表，形成GridTree上下级数据
        /// </summary>
        /// <param name="ParentModuleList">父级集合</param>
        /// <param name="TotalModuleList">数据总集合</param>
        /// <returns></returns>
        private string ResolveModuleGridStr(IEnumerable<ModuleTypeName> ParentModuleList, IEnumerable<ModuleTypeName> TotalModuleList)
        {
            StringBuilder ResultJson = new StringBuilder();
            ParentModuleList = ParentModuleList.OrderBy(o => o.SortIndex);
            int Total = ParentModuleList.Count();
            foreach (var o in ParentModuleList)
            {
                if (o.ParentId == 0)
                    ResultJson.Append("{\"ModuleId\":\"" + o.ModuleId + "\",\"Name\":\"" + o.Name + "\",\"ParentId\":\"" + o.ParentId + "\",\"ModuleValue\":\"" + o.ModuleValue + "\",\"LinkUrl\":\"" + o.LinkUrl + "\",\"TypeId\":\"" + o.TypeId + "\",\"TypeName\":\"" + o.TypeName + "\",\"KeyWords\":\"" + o.KeyWords + "\",\"Description\":\"" + o.Description + "\",\"SortIndex\":\"" + o.SortIndex + "\",\"Title\":\"" + o.Title + "\",\"ModuleKey\":\"" + (o.ModuleKey != null ? o.ModuleKey.Trim() : "") + "\",\"IsUsable\":\"" + o.IsUsable + "\",\"Icon\":\"" + o.Icon + "\",\"iconCls\":\"" + o.Icon + "\"}");
                else
                    ResultJson.Append("{\"ModuleId\":\"" + o.ModuleId + "\",\"Name\":\"" + o.Name + "\",\"ParentId\":\"" + o.ParentId + "\",\"ModuleValue\":\"" + o.ModuleValue + "\",\"LinkUrl\":\"" + o.LinkUrl + "\",\"TypeId\":\"" + o.TypeId + "\",\"TypeName\":\"" + o.TypeName + "\",\"KeyWords\":\"" + o.KeyWords + "\",\"Description\":\"" + o.Description + "\",\"SortIndex\":\"" + o.SortIndex + "\",\"Title\":\"" + o.Title + "\",\"ModuleKey\":\"" + (o.ModuleKey != null ? o.ModuleKey.Trim() : "") + "\",\"IsUsable\":\"" + o.IsUsable + "\",\"Icon\":\"" + o.Icon + "\",\"_parentId\":\"" + o.ParentId + "\"}");
                var ChildrenData = TotalModuleList.Where(t => t.ParentId == o.ModuleId);
                if (ChildrenData.Count() > 0)
                {
                    ResultJson.Append(",");
                    ResultJson.Append(ResolveModuleGridStr(ChildrenData, TotalModuleList));
                }
                if (1 < Total)
                    ResultJson.Append(",");
                Total--;
            }
            return ResultJson.ToString();
        }
        /// <summary>
        /// 获取模块comboxtree组件的json数据
        /// </summary>
        /// <returns>json</returns>
        public string GetModuleComboTreeJson()
        {
            StringBuilder ResultJson = new StringBuilder();
            var ModuleList = Context.Instance.Query<Module>("select ModuleId,Name,ParentId,Icon,SortIndex from Sys_Module where IsUsable=@0", true).OrderBy(o => o.SortIndex).ToList();
            ResultJson.Append("[{\"id\":\"0\",\"text\":\"模块根[非实际模块]\",\"children\":[");
            ResultJson.Append(ResolveModuleComboTreeStr(ModuleList.Where(o => o.ParentId == 0), ModuleList));
            ResultJson.Append("]}]");
            return ResultJson.ToString();
        }
        /// <summary>
        /// 获取模块comboxtree组件的json数据(不含根模块)
        /// </summary>
        /// <returns>json</returns>
        public string GetModuleNoRootComboTreeJson()
        {
            StringBuilder ResultJson = new StringBuilder();
            var ModuleList = Context.Instance.Query<Module>("select ModuleId,Name,ParentId,Icon,SortIndex from Sys_Module where IsUsable=@0", true).OrderBy(o => o.SortIndex).ToList();
            ResultJson.Append("[");
            ResultJson.Append(ResolveModuleComboTreeStr(ModuleList.Where(o => o.ParentId == 0), ModuleList));
            ResultJson.Append("]");
            return ResultJson.ToString();
        }
        /// <summary>
        /// 获取module comboxTree组件数据
        /// </summary>
        /// <param name="ParentModuleList"></param>
        /// <param name="TotalModuleList"></param>
        /// <returns></returns>
        private string ResolveModuleComboTreeStr(IEnumerable<Module> ParentModuleList, IEnumerable<Module> TotalModuleList)
        {
            StringBuilder ResultJson = new StringBuilder();
            ParentModuleList = ParentModuleList.OrderBy(o => o.SortIndex);
            int Count = 0;
            int Total = ParentModuleList.Count();
            foreach (var o in ParentModuleList)
            {
                ResultJson.Append("{\"id\":\"" + o.ModuleId + "\",\"text\":\"" + o.Name + "\"");
                if (!string.IsNullOrEmpty(o.Icon))
                    ResultJson.Append(",\"iconCls\":\"" + o.Icon + "\"");
                var ChildrenData = TotalModuleList.Where(t => t.ParentId == o.ModuleId);
                if (ChildrenData.Count() > 0)
                {
                    ResultJson.Append(",\"state\":\"closed\",\"children\":[");
                    ResultJson.Append(ResolveModuleComboTreeStr(ChildrenData, TotalModuleList) + "]");
                }
                ResultJson.Append("}");
                Count++;
                if (Count < Total)
                    ResultJson.Append(",");
            }
            return ResultJson.ToString();
        }
        /// <summary>
        /// 获取用于生产下拉框的TypeId数据
        /// </summary>
        /// <returns></returns>
        public IEnumerable<KeyValue> GetModuelTypeCombo()
        {
            var result = DictOp.GetKeyValuesByCode("M_Type");
            return result;
        }
        /// <summary>
        /// 获取当前角色的所有模块信息
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Module> GetRoleModule(int roleId)
        {
            return Context.Instance.Query<Module>("select ModuleId,Name,ParentId,linkurl,Icon,SortIndex from Sys_Module where ModuleId in(select distinct a.ModuleId from Sys_RoleLimit a inner join Sys_Role b on a.RoleId=@0 and b.IsUsable is TRUE and a.RoleId=b.RoleId) order by SortIndex ASC", roleId);
        }
        /// <summary>
        /// 根据用户角色获取导航菜单json数据
        /// </summary>
        /// <returns></returns>
        public string GetRoleModuleJson(int roleId)
        {
            List<Module> MenuList = GetRoleModule(roleId).ToList();
            var TopMenus = MenuList.Where(o => o.ParentId == 0);
            return ResolveIndexModules(TopMenus, MenuList);
        }
        /// <summary>
        /// 分解角色的Modulelist，生成桌面菜单
        /// </summary>
        /// <param name="MenuList">需要分解的模块集</param>
        /// <param name="TotalMenu">所有模块信息</param>
        /// <returns></returns>
        private string ResolveIndexModules(IEnumerable<Module> MenuList, IEnumerable<Module> TotalMenu)
        {
            StringBuilder MenuJsonResult = new StringBuilder();
            int x = 1;
            int y = 0;
            MenuJsonResult.Append("[");
            foreach (var o in MenuList.ToList())
            {
                var ChildMenu = TotalMenu.Where(t => t.ParentId == o.ModuleId).OrderBy(t => t.SortIndex);
                if (ChildMenu.Count() > 0)
                {
                    MenuJsonResult.Append("{id:'" + o.ModuleId + "',name:'" + o.Name + "',icon:'" + o.Icon + "',url:'',menus:");
                    MenuJsonResult.Append(ResolveIndexModules(ChildMenu, TotalMenu));
                    MenuJsonResult.Append("}");
                    y++;
                }
                else
                    MenuJsonResult.Append("{id:'" + o.ModuleId + "',name:'" + o.Name + "',icon:'" + o.Icon + "',url:'" + o.LinkUrl + "'}");
                if (x < MenuList.Count())
                    MenuJsonResult.Append(",");
                x++;
            }
            MenuJsonResult.Append("]");
            return MenuJsonResult.ToString();
        }
        /// <summary>
        /// 根据模块编号删除指定模块信息
        /// </summary>
        /// <param name="ModuleId">模块编号</param>
        /// <returns></returns>
        public int DeleteModuleById(int ModuleId)
        {
            var DataContext = Context.Instance;
            var count = 0;
            DataContext.BeginTransaction();
            try
            {
                count += DataContext.Delete<SysRoleLimit>("where ModuleId=@0", ModuleId);
                count += DataContext.Delete<Limit>("where ModuleId=@0", ModuleId);
                count += DataContext.Delete<Module>("where ModuleId=@0", ModuleId);
                DataContext.CompleteTransaction();
            }
            catch
            {
                DataContext.AbortTransaction();
            }
            return count;
        }
        /// <summary>
        /// 根据模块key获取模块SEO信息
        /// </summary>
        /// <param name="modeuleKey">模块key</param>
        /// <returns></returns>
        public SEOInfo GetSEOInfoByKey(string modeuleKey)
        {
            return CacheOp.GetOrAdd<SEOInfo>("SEO:" + modeuleKey, () =>
            {
                return Context.Instance.SingleOrDefault<SEOInfo>("SELECT keywords,description,title from sys_module where modulekey=@0", modeuleKey);
            });
        }
        /// <summary>
        /// 通过上级Key获取下级列表
        /// </summary>
        /// <param name="key">上级ModuleKey</param>
        /// <returns></returns>
        public List<Module> GetListByParentKey(string key)
        {
            return Context.Instance.Query<Module>("SELECT name,linkurl from sys_module WHERE parentid=(SELECT moduleid from sys_module WHERE modulekey=@0 LIMIT 1) and isusable=true order by sortindex asc", key).ToList();
        }
    }
}
