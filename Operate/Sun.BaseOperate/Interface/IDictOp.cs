using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;
using Sun.Model.DB;
using Sun.Model.Common;

namespace Sun.BaseOperate.Interface
{
    public interface IDictOp:IDBOperate<Dict>
    {
        /// <summary>
        /// 获取字典的分页数据集
        /// </summary>
        /// <param name="typeId"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        Page<Dict> GetDictPageList(int typeId, int page, int rows);
        /// <summary>
        /// 获取用作下拉框的字典数据集
        /// </summary>
        /// <param name="code">字典子集所属组的编码</param>
        /// <returns></returns>
        IEnumerable<KeyValue> GetKeyValuesByCode(string code);
        /// <summary>
        /// 获取用作下拉框的字典数据集
        /// </summary>
        /// <param name="code">字典子集所属组的编码</param>
        /// <returns></returns>
        List<Dict> GetListByCode(string code);
        /// <summary>
        /// 获取字典类别数据集
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> GetTypeKeyValues();
        /// <summary>
        /// 删除字典类型
        /// </summary>
        /// <param name="typeId">类型Id</param>
        /// <returns></returns>
        int DeleteDictType(int typeId);
        /// <summary>
        /// 根据数值获取字典信息
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        List<KeyValue> GetDictByValue(string value);
    }
}
