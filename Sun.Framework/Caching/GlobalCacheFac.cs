using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Caching;

namespace Sun.Framework.Caching
{
    public class GlobalCacheFac : IGlobalCacheFac
    {
        ICache Cache { get; set; }
        ICacheOp CacheOp { get; set; }
        public GlobalCacheFac(ICacheOp cacheOp, ICache cache)
        {
            Cache = cache;
            CacheOp = cacheOp;
        }
        public void UpdateByKey(string key)
        {
            Cache.Delete(key);
        }

        public void UpdateComplate(string key)
        {
            var array = key.Split('|');
            string entityType = array[0], keyId = array[1], keyValue = array[2];
            CacheOp.ComplateUpdate(entityType, keyId, keyValue, DateTime.Now);
        }

        public void UpdateAll(DateTime? updateTime = null)
        {
            foreach (var key in GetNeedUpdateKeyList())
            {
                UpdateByKey(key);
            }
        }

        public IEnumerable<string> GetNeedUpdateKeyList()
        {
            return CacheOp.GetNeedUpdateList();
        }
    }
}
