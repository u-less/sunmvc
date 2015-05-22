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
    public interface IConfOptionOp:IDBOperate<ConfigOption>
    {
        /// <summary>
        /// 获取网站配置内容项的分页数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">单页数据量</param>
        /// <param name="group">组别</param>
        /// <returns></returns>
        Page<ConfigOptionGrid> GetPageList(int page, int rows, int groupId = 0, int configId = 0, string optionName = null);
        /// <summary>
        /// 获取网站配置内容项的所有数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<ConfigOption> GetOptionList();
        /// <summary>
        /// 通过配置内容项Id获取配置项信息
        /// </summary>
        /// <param name="optionId">配置内容项Id</param>
        /// <returns></returns>
        ConfigOption GetOptionById(int optionId);
        /// <summary>
        /// 通过配置内容项Id获取配置项信息
        /// </summary>
        /// <param name="optionId">配置内容项Id</param>
        /// <returns></returns>
        ConfigOptionGrid GetOptionGridById(int optionId);
    }
}
