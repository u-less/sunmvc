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
    [IocExport(typeof(IDictOp), true)]
    public class DictOp : IDictOp
    {
        public IModelCacheFac<Dict> CacheOp
        {
            get;
            set;
        }
         public DictOp(IModelCacheFac<Dict> cacheOp)
        {
            CacheOp = cacheOp;
        }
        public Core.DBContext.Page<Dict> GetDictPageList(int typeId, int page, int rows)
        {
            return Context.Instance.Page<Dict>(page, rows, "select * from Sys_Dict where TypeId=@0 order by SortIndex ASC", typeId);
        }

        public IEnumerable<KeyValue> GetKeyValuesByCode(string code)
        {
            return CacheOp.GetOrAdd<List<KeyValue>>(CacheOp.BuildKey("kv-code", code),
                () =>
                {
                    return Context.Instance.Query<Dict>("select DictId,DictName from Sys_Dict where TypeId=(select DictId from Sys_Dict where Code=@0 LIMIT 1) and IsUsable='True' order by SortIndex ASC", code)
                    .Select(o => new KeyValue { key = o.DictId.ToString(), value = o.DictName }).ToList();
                });
        }

        public List<Dict> GetListByCode(string code)
        {
            return CacheOp.GetOrAdd<List<Dict>>(CacheOp.BuildKey("list-code", code), () =>
            {
                return Context.Instance.Query<Dict>("select * from Sys_Dict where TypeId=(select DictId from Sys_Dict where Code=@0 LIMIT 1) and IsUsable='True' order by SortIndex ASC", code).ToList();
            });
        }

        public IEnumerable<KeyValue> GetTypeKeyValues()
        {
            return Context.Instance.Query<Dict>("select DictId,DictName from Sys_Dict where TypeId=0").Select(o => new KeyValue { key = o.DictId.ToString(), value = o.DictName });
        }

        public int DeleteDictType(int typeId)
        {
            throw new NotImplementedException();
        }

        public List<KeyValue> GetDictByValue(string value)
        {
            return CacheOp.GetOrAdd<List<KeyValue>>(CacheOp.BuildKey("kv-value", value), () =>
            {
                return Context.Instance.Query<Dict>("select DictId,DictName from Sys_Dict where datavalue=@0 and IsUsable='True' order by SortIndex ASC", value).Select(o => new KeyValue { key = o.DictId.ToString(), value = o.DictName }).ToList();
            });
        }

        public object Add(Dict entity)
        {
            var r = Convert.ToInt32(Context.Instance.Insert(entity));
            if (r > 0) CacheOp.UpdateAll();
            return r;
        }

        public int Delete(object id)
        {
            var r = Context.Instance.Delete<Dict>(id);
            if (r > 0) CacheOp.UpdateAll();
            return r;
        }

        public int Update(Dict entity)
        {
            var r = Context.Instance.Update(entity);
            if (r > 0) CacheOp.UpdateAll();
            return r;
        }

        public int Update(Dict entity, string[] columns)
        {
            var r = Context.Instance.Update(entity,columns);
            if (r > 0) CacheOp.UpdateAll();
            return r;
        }

        public Dict GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<Dict>(id);
        }
    }
}
