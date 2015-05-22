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
    [IocExport(typeof(IOrganOp), true)]
    public class OrganOp:IOrganOp
    {
        public IModelCacheFac<Organ> CacheOp
        {
            get;
            set;
        }
        IDictOp DictOp;
        public OrganOp(IModelCacheFac<Organ> cacheOp,IDictOp dictOp)
        {
            CacheOp = cacheOp;
            DictOp = dictOp;
        }
        public Page<OrganGrid> GetPageList(int page = 1, int rows = 30, string typeId = null, string level = null, string organName = null, string parentIds = null, bool haveme = false)
        {
            var sqlstr = "SELECT o.*,b.dictname typename,d.dictname levelname,e.organname parentname from sys_organ o INNER JOIN sys_dict b on o.typeid=b.dictid INNER JOIN sys_dict d on o.level=d.dictid LEFT JOIN sys_organ e on o.parentid=e.organid";
            Sql sql = new Sql(sqlstr);
            if (!string.IsNullOrEmpty(typeId))
            {
                sql.Where("o.typeid=@0", typeId);
            }
            if (!string.IsNullOrEmpty(level))
            {
                sql.Where("o.level=@0", level);
            }
            if (!string.IsNullOrEmpty(parentIds) && !haveme)
            {
                sql.Where("regexp_split_to_array('" + parentIds + "',',')@> array[o.parentid]::text[]");
            }
            if (!string.IsNullOrEmpty(parentIds) && haveme)
            {
                var pids = parentIds.Split(',');
                string or = null;
                for (int i = 0; i < pids.Length; i++)
                {
                    or += " or o.organid=@" + i;
                }
                sql.Where("(regexp_split_to_array('" + parentIds + "',',')@> array[o.parentid]::text[]" + or + ")", pids);
            }
            if (!string.IsNullOrEmpty(organName))
            {
                sql.Where("o.organname LIKE @0", "%" + organName.Trim() + "%");
            }
            sql.OrderBy("o.parentid", "o.organid");
            return Context.Instance.Page<OrganGrid>(page, rows, sql);
        }

        public string CreateOrganId(string parentId)
        {
            string strSql = "SELECT organid from sys_organ WHERE parentid=@0 ORDER BY organid DESC LIMIT 1";
            string organId = null;
            var maxId = Context.Instance.ExecuteScalar<string>(strSql, parentId);
            if (!string.IsNullOrEmpty(maxId))
            {
                organId = (Convert.ToInt32(maxId) + 1).ToString();
            }
            else
            {
                switch (parentId.Length)
                {
                    case 1: organId = "000001"; break;
                    case 8: organId = "01"; break;
                    default: { return null; }
                }
                organId = parentId + organId;
            }
            return organId;
        }

        public IEnumerable<KeyValue> GetTypes()
        {
            return DictOp.GetKeyValuesByCode("O_Type");
        }

        public IEnumerable<KeyValue> GetLevels()
        {
            return DictOp.GetKeyValuesByCode("O_Level");
        }

        public bool IsParent(string organId)
        {
            if (String.IsNullOrEmpty(organId))
                return false;
            string sql = "select count(OrganId) from Sys_Organ where parentid=@0";
            return Context.Instance.ExecuteScalar<int>(sql, organId) > 0;
        }

        public object Add(Organ entity)
        {
            return Context.Instance.Insert(entity);
        }

        public int Delete(object id)
        {
            return Context.Instance.Delete<Organ>(id);
        }

        public int Update(Organ entity)
        {
            return Context.Instance.Update(entity);
        }

        public int Update(Organ entity, string[] columns)
        {
            return Context.Instance.Update(entity,columns);
        }

        public Organ GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<Organ>(id);
        }
    }
}
