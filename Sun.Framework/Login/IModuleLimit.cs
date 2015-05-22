using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Framework.Login
{
    public interface IModuleLimit
    {        /// <summary>
        /// 通过模块key获取指定角色的权限
        /// </summary>
        /// <param name="moduleKey">模块标识</param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        List<int> GetLimitsByKey(string moduleKey, string roleId);
        /// <summary>
        /// 获取模块值
        /// </summary>
        /// <param name="moduleKey">模块标识</param>
        /// <returns></returns>
        string GetModuleValue(string moduleKey); 
    }
}
