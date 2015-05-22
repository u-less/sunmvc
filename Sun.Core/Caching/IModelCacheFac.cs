using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Caching
{
    public interface IModelCacheFac<T> where T :class
    {
        string EntityType{ get;}
        string BuildKey(string keyId, params string[] keyValues);
        void UpdateByKey(string key);
        void UpdateById(string keyId, DateTime? updateTime = null);
        TData GetOrAdd<TData>(string key, Func<TData> getFunc,Func<bool> filter=null) where TData : class;
        void UpdateComplateByKey(string key);
        void UpdateAll(DateTime? updateTime = null);
    }
}
