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
using Autofac;

namespace Sun.BaseOperate.DB
{
    [IocExport(typeof(IRoleLimitOp), true)]
    public class RoleLimitOp:IRoleLimitOp
    {
        public RoleLimitOp(IModelCacheFac<SysRoleLimit> CacheOp)
        {
            this.CacheOp = CacheOp;
        }
        public string GetLimitAboutRoleJson(int roleId, int setRoleId)
        {
            IModuleOp ModuleOp = WebIoc.Container.Resolve<IModuleOp>();
            List<ModuleAoutRole> moduleList = GetModuleAboutRole(roleId, setRoleId).ToList();
            int x = moduleList.Count();
            List<LimitAoutRole> LimitList = GetLimitsAboutRole(roleId, setRoleId).ToList();
            StringBuilder ResultStr = new StringBuilder();
            ResultStr.Append("{'total': " + x + ", 'rows':[");
            foreach (var module in moduleList)
            {
                if (module.ParentId == 0)
                    ResultStr.Append("{'ModuleId':'" + module.ModuleId + "','Name':'" + module.Name + "','Select':'" + module.HasLimit + "',\"iconCls\":\"" + module.Icon + "\",'LimitList':[");
                else
                    ResultStr.Append("{'ModuleId':'" + module.ModuleId + "','Name':'" + module.Name + "','_parentId':'" + module.ParentId + "','Select':'" + module.HasLimit + "',\"iconCls\":\"" + module.Icon + "\",'LimitList':[");
                var DataList = LimitList.Where(t => t.ModuleId == module.ModuleId).OrderBy(o => o.Code);
                int y = DataList.Count();
                foreach (var o in DataList)
                {
                    ResultStr.Append("{'LimitId':'" + o.LimitId + "','ModuleId':'" + o.ModuleId + "','Name':'" + o.Name + "','Select':'" + o.HasLimit + "'}");
                    if (y > 1)
                        ResultStr.Append(",");
                    y--;
                }
                ResultStr.Append("]}");
                if (x > 1)
                    ResultStr.Append(",");
                x--;
            }
            ResultStr.Append("]}");
            return ResultStr.ToString();
        }

        public List<int> GetLimitsByModuleAndRole(string moduleKey, string roleId)
        {
            return CacheOp.GetOrAdd<List<int>>(CacheOp.BuildKey(moduleKey, roleId), () =>
            {
                string sql = "select b.Code from sys_roleLimit a inner join sys_limit b on b.moduleid=(SELECT ModuleId from sys_module where modulekey=@0 LIMIT 1) and a.RoleId=@1 and b.limitid=a.limitid";
                return DbContext.Context.Instance.Query<int>(sql, moduleKey, roleId).ToList();
            });
        }

        public IEnumerable<Limit> GetRoleLimits(int roleId)
        {
            return DbContext.Context.Instance.Query<Limit>(" select a.* from Sys_Limit a inner join Sys_RoleLimit b on a.LimitId=b.LimitId and b.RoleId=@0)", roleId);
        }

        public IEnumerable<LimitAoutRole> GetLimitsAboutRole(int roleId, int setRoleId = 0)
        {
            var sql = "select a.*,b.RLId as HasLimit from Sys_Limit a left join Sys_RoleLimit b on a.LimitId=b.LimitId and b.RoleId=@0";
            if (setRoleId != 0)
                sql += " where a.limitid in(SELECT limitid from sys_rolelimit WHERE roleid=@1)";
            return DbContext.Context.Instance.Query<LimitAoutRole>(sql, roleId, setRoleId);
        }

        public IEnumerable<ModuleAoutRole> GetModuleAboutRole(int roleId, int setRoleId = 0)
        {
            var sql = setRoleId == 0 ? "" : "where a.ModuleId in(SELECT DISTINCT(moduleid) FROM sys_rolelimit  WHERE LimitId=0 AND roleid=@1)";
            return DbContext.Context.Instance.Query<ModuleAoutRole>("select a.*,b.RLId as HasLimit from Sys_Module a left JOIN Sys_RoleLimit b on a.ModuleId=b.ModuleId and b.LimitId=0 and RoleId=@0 " + sql + " ORDER BY a.sortindex asc", roleId, setRoleId);
        }

        public bool RoleLimitExists(int roleId, int limitId)
        {
            var result = DbContext.Context.Instance.ExecuteScalar<int>("select count(1) from Sys_RoleLimit where RoleId=@0 and LimitId=@1", roleId, limitId);
            return result > 0;
        }

        public bool RoleModuleExists(int roleId, int moduleId)
        {
            var result = DbContext.Context.Instance.ExecuteScalar<int>("select count(1) from Sys_RoleLimit where RoleId=@0 and ModuleId=@1 and LimitId=0", roleId, moduleId);
            return result > 0;
        }

        public void SetRoleLimit(int roleId, string[] limitIds)
        {
            if (limitIds.Length > 0)
                foreach (var o in limitIds)
                {
                    if (o.IndexOf("#1") != -1)
                    {
                        var result = o.Split('#')[0];
                        var lm = result.Split('>');
                        var limitId = int.Parse(lm[0]);
                        var moduleId = int.Parse(lm[1]);
                        if (!RoleLimitExists(roleId, limitId))
                        {
                            SysRoleLimit roleLimit = new SysRoleLimit();
                            roleLimit.LimitId = limitId;
                            roleLimit.RoleId = roleId;
                            roleLimit.ModuleId = moduleId;
                            DbContext.Context.Instance.Insert(roleLimit);
                        }
                    }
                    else
                        DeleteRoleLimit(roleId, int.Parse(o));
                }
        }

        public void SetRoleModule(int roleId, string[] modules)
        {
            if (modules.Length > 0)
                foreach (var o in modules)
                {
                    if (o.IndexOf("#1") != -1)
                    {
                        var ModuleID = int.Parse(o.Split('#')[0]);
                        if (!RoleModuleExists(roleId, ModuleID))
                        {
                            SysRoleLimit roleLimit = new SysRoleLimit();
                            roleLimit.RoleId = roleId;
                            roleLimit.ModuleId = ModuleID;
                            roleLimit.LimitId = 0;
                            DbContext.Context.Instance.Insert(roleLimit);
                        }
                    }
                    else
                    {
                        DeleteRoleModule(roleId, int.Parse(o));
                    }
                }
        }

        public void DeleteRoleLimit(int roleId, int limitId)
        {
            DbContext.Context.Instance.Delete<SysRoleLimit>("where RoleId=@0 and LimitId=@1", roleId, limitId);
        }

        public void DeleteRoleModule(int roleId, int moduleId)
        {
            DbContext.Context.Instance.Delete<SysRoleLimit>("where RoleId=@0 and ModuleId=@1 and LimitId=0", roleId, moduleId);
        }

        public IModelCacheFac<SysRoleLimit> CacheOp
        {
            get;
            set;
        }

        public object Add(SysRoleLimit entity)
        {
            return DbContext.Context.Instance.Insert(entity);
        }

        public int Delete(object id)
        {
            return DbContext.Context.Instance.Delete(id);
        }

        public int Update(SysRoleLimit entity)
        {
            return DbContext.Context.Instance.Update(entity);
        }

        public int Update(SysRoleLimit entity, string[] columns)
        {
            return DbContext.Context.Instance.Update(entity, columns);
        }

        public SysRoleLimit GetModelById(object id)
        {
            return DbContext.Context.Instance.SingleOrDefault<SysRoleLimit>(id);
        }
    }
}
