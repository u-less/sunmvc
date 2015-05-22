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
using Sun.Core.DBContext;

namespace Sun.BaseOperate.DB
{
    [IocExport(typeof(IRoleOp), true)]
    public class RoleOp:IRoleOp
    {
         public IModelCacheFac<Role> CacheOp
        {
            get;
             set;
        }
         public RoleOp(IModelCacheFac<Role> cacheOp)
        {
            CacheOp = cacheOp;
        }
        public Page<Model.DBExtensions.RoleGrid> GetPageList(int page, int rows)
        {
            return Context.Instance.Page<RoleGrid>(page, rows, "select a.*,b.UserName as AdminName,d.organname from sys_role a left join Sys_UserInfo b on b.UserId=a.AdminId LEFT JOIN sys_organ d on a.organid=d.organid order by a.SortIndex ASC");
        }

        public IEnumerable<KeyValue> RoleKeyValues()
        {
            return Context.Instance.Query<Role>("select RoleId,Name from Sys_Role where IsUsable='true'").Select(o => new KeyValue { key = o.RoleId.ToString(), value = o.Name });
        }

        public object Add(Role entity)
        {
            return Context.Instance.Insert(entity);
        }

        public int Delete(object id)
        {
            return Context.Instance.Delete<Role>(id);
        }

        public int Update(Role entity)
        {
            return Context.Instance.Update(entity);
        }

        public int Update(Role entity, string[] columns)
        {
            return Context.Instance.Update(entity,columns);
        }

        public Role GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<Role>(id);
        }
    }
}
