using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Session
{
    public interface ISession
    {
        /// <summary>
        /// 索引属性
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>session值</returns>
        string this[string key] { get; set; }
        /// <summary>
        /// 设置session
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        void Set(string key, string value, TimeSpan? expire = null);
        /// <summary>
        /// 设置session对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        /// <param name="expire">过期时间</param>
        void Set<T>(string key, T value, TimeSpan? expire = null) where T : class;
        /// <summary>
        /// 获取session
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        string Get(string key);
        /// <summary>
        /// 获取session对象
        /// </summary>
        /// <typeparam name="T">对象类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        T Get<T>(string key) where T : class;
        /// <summary>
        /// 删除session
        /// </summary>
        /// <param name="key">键</param>
        void Delete(string key);
    }
}
