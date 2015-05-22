using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;
using Sun.Model.DB;
using Sun.Model.DBExtensions;
using Sun.Model.Common;

namespace Sun.BaseOperate.Interface
{
    public interface IOrganOp:IDBOperate<Organ>
    {
        Page<OrganGrid> GetPageList(int page = 1, int rows = 30, string typeId = null, string level = null, string organName = null, string parentIds = null, bool haveme = false);
        /// <summary>
        /// 自动生成下级机构的Id
        /// </summary>
        /// <param name="parentId"></param>
        /// <returns></returns>
        string CreateOrganId(string parentId);
        /// <summary>
        /// 获取机构类别简单数据列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> GetTypes();
        /// <summary>
        /// 获取机构等级简单数据列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> GetLevels();
        /// <summary>
        /// 判断是否是父级机构
        /// </summary>
        /// <param name="organId"></param>
        /// <returns></returns>
        bool IsParent(string organId);
    }
}
