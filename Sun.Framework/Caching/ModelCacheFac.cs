using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Caching;

namespace Sun.Framework.Caching
{
    public class ModelCacheFac<T> : IModelCacheFac<T> where T : class
    {
        private System.Type type;
        ICache Cache { get; set; }
        ICacheOp CacheOp { get; set; }
        public ModelCacheFac(ICacheOp cacheOp, ICache cache)
        {
            type = typeof(T);
            Cache = cache;
            CacheOp = cacheOp;

        }
        public string EntityType
        {
            get { return type.Name; }
        }

        public string BuildKey(string keyId, params string[] keyValues)
        {
            var build = new StringBuilder(100).Append(EntityType).Append("|").Append(keyId).Append("|");
            for (int i = 0; i < keyValues.Length; i++)
            {
                if (i > 0) build.Append(":");
                build.Append(keyValues[i]);
            }
            return build.ToString();
        }

        public void UpdateByKey(string key)
        {
            Cache.Delete(key);
        }

        public void UpdateById(string keyId, DateTime? updateTime = null)
        {
            var upTime = updateTime ?? DateTime.Now;
            CacheOp.SetNeedUpdate(EntityType, upTime, keyId);
        }

        public DataType GetOrAdd<DataType>(string key, Func<DataType> getFunc, Func<bool> filter = null) where DataType : class
        {
            if (null == filter || filter() == true)
            {
                DataType data = default(DataType);
                var s = Cache.Get<DataType>(key, out data);
                if (!s)
                {
                    data = getFunc();
                    Cache.Set<DataType>(key, data, TimeSpan.FromMinutes(30));
                    UpdateComplateByKey(key);
                }
                return data;
            }
            else {
                return getFunc();
            }
        }

        public void UpdateComplateByKey(string key)
        {
            var array = key.Split('|');
            string entityType = array[0], keyId = array[1], keyValue = array[2];
            CacheOp.ComplateUpdate(entityType, keyId, keyValue, DateTime.Now);
        }

        public void UpdateAll(DateTime? updateTime = null)
        {
            var upTime = updateTime ?? DateTime.Now;
            CacheOp.SetNeedUpdate(EntityType, upTime);
        }
    }
}
