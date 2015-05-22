using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Framework.Calculate;
using Sun.Core.Caching;
using Sun.Core.Ioc;
using Autofac;

namespace Sun.Framework.Security
{
    public static class SecretKey
    {
        private const string keyPrefix = "u_s_k_";//key的前缀
        /// <summary>
        /// 获取加密key
        /// </summary>
        /// <param name="ticks">标识</param>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public static string GetKey(long ticks,string userId, TimeSpan? expir = null)
        {
            var cache = WebIoc.Container.Resolve<ICache>();
            var ckey = keyPrefix+ticks+"_"+ userId;
            string key;
            if (!cache.StringGet(ckey, out key))
            {
                key =RandomHelper.Str(8);
                cache.StringSet(ckey, key, expir);
            }
            return key;
        }
        /// <summary>
        /// 删除key
        /// </summary>
        /// <param name="ticks">标识</param>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public static void KeyDelete(int ticks, string userId)
        {
            var ckey = keyPrefix + ticks + "_" + userId;
            WebIoc.Container.Resolve<ICache>().Delete(ckey);
        }
    }
}
