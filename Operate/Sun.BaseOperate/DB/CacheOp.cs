using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Model.DB;
using Sun.BaseOperate.DbContext;
using Sun.Framework.Caching;
using Sun.Core.Ioc;

namespace Sun.BaseOperate.DB
{
  [IocExport(typeof(ICacheOp), true)]
   public class CacheOp:ICacheOp
    {
        /// <summary>
        /// 设置指定类型的数据需要立即更新(立即更新)
        /// </summary>
        /// <param name="cachetype"></param>
        /// <returns></returns>
       public bool SetNeedUpdate(string cachetype, DateTime updateTime, string keyId = null, string keyValue = null)
       {
           var sql = "set bookupdatetime=@0 where cachetype=@1";
           if (!string.IsNullOrEmpty(keyId))
               sql += " and keyid=@2";
           if (!string.IsNullOrEmpty(keyValue))
               sql += " and keyvalue=@3";
           return Context.Instance.Update<CacheInfo>(sql, updateTime, cachetype, keyId, keyValue) > 0;
       }
        /// <summary>
        /// 设置缓存更新完成
        /// </summary>
        /// <param name="cachetype">实体类型</param>
        /// <param name="key">键值</param>
        /// <param name="updateTime">完成数据更新的时间</param>
        /// <returns></returns>
        public bool ComplateUpdate(string cachetype, string keyId, string keyValue, DateTime updateTime)
        {
            var context = Context.Instance;
            var count = context.ExecuteScalar<int>("select count(*) from cacheinfo where cachetype=@0 and keyid=@1 and keyvalue=@2", cachetype, keyId, keyValue);
            if (count > 0)
            {
                return context.Update<CacheInfo>("set UpdateTime=@0 where cachetype=@1 and keyid=@2 and keyvalue=@3", updateTime, cachetype, keyId, keyValue) > 0;
            }
            else
            {
                var cache = new CacheInfo { CacheType = cachetype, KeyId = keyId, KeyValue = keyValue, UpdateTime = updateTime, BookUpdateTime = updateTime };
                return Convert.ToInt32(context.Insert(cache)) > 0;
            }
        }
        /// <summary>
        /// 获取所有数据更新内容，用在首次获取缓存信息至内存
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetNeedUpdateList()
        {
            return Context.Instance.Query<CacheInfo>("select concat_WS('|',cachetype,keyId,keyvalue)keyvalue from cacheinfo WHERE bookupdatetime>updatetime AND bookupdatetime<=@0", DateTime.Now).Select(c => c.KeyValue);
        }
    }
}
