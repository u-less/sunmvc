using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Framework.Caching
{
    public interface ICacheOp
    {
        /// <summary>
        /// 设置指定类型的数据需要更新(立即更新)
        /// </summary>
        /// <param name="cachetype"></param>
        /// <returns></returns>
        bool SetNeedUpdate(string cachetype, DateTime updateTime, string keyId = null, string keyValue = null);
         /// <summary>
        /// 设置缓存更新完成
        /// </summary>
        /// <param name="cachetype">实体类型</param>
        /// <param name="key">键值</param>
        /// <param name="updateTime">完成数据更新的时间</param>
        /// <returns></returns>
        bool ComplateUpdate(string cachetype, string keyId, string keyValue, DateTime updateTime);
        /// <summary>
        /// 获取所有数据更新内容，用在首次获取缓存信息至内存
        /// </summary>
        /// <returns></returns>
        IEnumerable<string> GetNeedUpdateList();
    }
}
