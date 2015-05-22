using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;
using Sun.Model.DB;
using Sun.Model.Common;
using Sun.Model.DBExtensions;

namespace Sun.BaseOperate.Interface
{
    public interface ILimitOp:IDBOperate<Limit>
    {
        /// <summary>
        /// 获取权限分页信息（附带模块名称）
        /// </summary>
        /// <param name="ModuleID">模块Id</param>
        /// <param name="page">页码</param>
        /// <param name="rows">每页条数</param>
        /// <returns></returns>
        Page<LimitModuleName> GetPageList(int moduleId, int page, int rows);
        /// <summary>
        /// 判断key是否已经存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool KeyExits(int key, int moduleId, int id = 0);
    }
}
