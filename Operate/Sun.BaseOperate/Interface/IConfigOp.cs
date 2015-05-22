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
    public interface IConfigOp:IDBOperate<ConfigInfo>
    {
        /// <summary>
        /// 获取网站配置项的分页数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="rows">单页数据量</param>
        /// <param name="group">组别</param>
        /// <returns></returns>
        Page<ConfigGrid> GetPageList(int page, int rows, int group = 0, int opType = -1, string confName = null);
        /// <summary>
        /// 获取所有配置项数据
        /// </summary>
        /// <returns></returns>
        List<ConfigSet> GetConfList();
        /// <summary>
        /// 配置组列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> GetGroupList();
        /// <summary>
        /// 通过Key获取配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        ConfigInfo GetModelByKey(string key);
        /// <summary>
        /// 查看配置的key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        bool KeyExits(string key);
        /// <summary>
        /// 通过Key设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value">值</param>
        /// <returns></returns>
        bool SetValueByKey(int confId, string value);
        /// <summary>
        /// 通过配置组编号获取该组的配置列表数据
        /// </summary>
        /// <param name="groupid"></param>
        /// <returns></returns>
        IEnumerable<KeyValue> GetListByGroupId(int groupid);
        /// <summary>
        /// 通过配置key获取多选配置的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns>list类value</returns>
        List<string> GetCheckConfigValueByKey(string key);
        /// <summary>
        /// 通过配置key获取单选选配置的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns>list类value</returns>
        string GetRadioConfigValueByKey(string key);
        /// <summary>
        /// 通过配置key获取配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns>字符串</returns>
        string GetConfigValueByKey(string key);
        /// <summary>
        /// 通过配置key获取配置信息
        /// </summary>
        /// <param name="key"></param>
        /// <returns>对象</returns>
        T GetConfigObjectByKey<T>(string key);
        /// <summary>
        /// 清除配置缓存数据
        /// </summary>
        /// <param name="key">配置key</param>
        /// <returns></returns>
        bool ClearConfigCache(string key = null);
    }
}
