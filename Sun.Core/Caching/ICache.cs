using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Caching
{
    public interface ICache
    {
        /// <summary>
        /// 实体数据写入缓存
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">主键</param>
        /// <param name="Entity">数据</param>
        /// <param name="expiry">过期时间</param>
        void Set<T>(string key, T Entity, TimeSpan? expiry = null);
        /// <summary>
        /// 获取单条数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        bool Get<T>(string key, out T entity) where T : class;
        /// <summary>
        /// 字符串缓存
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expiry">过期时间</param>
        void StringSet(string key, string value, TimeSpan? expiry = null);
        /// <summary>
        /// 字符串获取
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool StringGet(string key, out string value);
        /// <summary>
        /// 移除数据
        /// </summary>
        /// <param name="key">主键</param>
        void Delete(string key);
        /// <summary>
        /// 设置缓存过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="expire">过期时间</param>
        void Expire(string key, TimeSpan expire);
    }
}
