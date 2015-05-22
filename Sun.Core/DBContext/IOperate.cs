using System;
using Sun.Core.Caching;

namespace Sun.Core.DBContext
{
    public interface IDBOperate<T> where T : class
    {
        IModelCacheFac<T> CacheOp { get; set; }
        object Add(T entity);
        int Delete(object id);

        int Update(T entity);

        int Update(T entity, string[] columns);

        T GetModelById(object id);
    }
}
