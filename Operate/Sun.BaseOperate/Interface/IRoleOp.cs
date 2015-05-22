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
    public interface IRoleOp:IDBOperate<Role>
    {
        /// <summary>
        /// 获取role分页数据列表
        /// </summary>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        Page<RoleGrid> GetPageList(int page, int rows);
        /// <summary>
        /// 获取角色的ID及名字列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> RoleKeyValues();
    }
}
