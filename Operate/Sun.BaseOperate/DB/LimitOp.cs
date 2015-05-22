using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Caching;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.Model.DBExtensions;
using Sun.BaseOperate.DbContext;
using Sun.Core.Ioc;

namespace Sun.BaseOperate.DB
{
    [IocExport(typeof(ILimitOp), true)]
    public class LimitOp:ILimitOp
    {
        public IModelCacheFac<Limit> CacheOp
        {
            get;
            set;
        }
        public LimitOp(IModelCacheFac<Limit> cacheOp)
        {
            CacheOp = cacheOp;
        }
        public Core.DBContext.Page<LimitModuleName> GetPageList(int moduleId, int page, int rows)
        {
            string sql = null;
            if (moduleId != 0)
                sql = "select a.*,b.Name as ModuleName from Sys_Limit a inner join Sys_Module b on a.ModuleId=b.ModuleId and a.ModuleId=@0";
            else
                sql = "select *,'通用权限' as ModuleName from Sys_Limit where ModuleId=@0";
            var result =Context.Instance.Page<LimitModuleName>(page, rows, sql, moduleId);
            return result;
        }

        public bool KeyExits(int key, int moduleId, int id = 0)
        {
            var sql = "select code from Sys_Limit where code=@0 and moduleid=@1";
            if (id != 0)
                sql += " and limitid<>@2";
            return Context.Instance.SingleOrDefault<int>(sql, key, moduleId, id) > 0;
        }

        public object Add(Limit entity)
        {
            return Context.Instance.Insert(entity);
        }

        public int Delete(object id)
        {
            return Context.Instance.Delete<Limit>(id);
        }

        public int Update(Limit entity)
        {
            return Context.Instance.Update(entity);
        }

        public int Update(Limit entity, string[] columns)
        {
            return Context.Instance.Update(entity, columns);
        }

        public Limit GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<Limit>(id);
        }
    }
}
