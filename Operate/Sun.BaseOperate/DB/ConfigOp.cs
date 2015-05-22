using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Sun.Core.Caching;
using Sun.BaseOperate.Interface;
using Sun.Model.Common;
using Sun.Model.DB;
using Sun.Model.DBExtensions;
using Sun.BaseOperate.DbContext;
using Sun.Core.Ioc;
using Newtonsoft.Json;

namespace Sun.BaseOperate.DB
{
    [IocExport(typeof(IConfigOp), true)]
    public class ConfigOp:IConfigOp
    {
        /// <summary>
        /// 配置信息缓存器(结果为列表)
        /// </summary>
        private static ConcurrentDictionary<string, List<string>> ConfigListValues = new ConcurrentDictionary<string, List<string>>();
        /// <summary>
        /// 配置信息缓存器(结果为字符串)
        /// </summary>
        private static ConcurrentDictionary<string, string> ConfigValues = new ConcurrentDictionary<string, string>();
        /// <summary>
        /// 配置信息缓存器(结果为对象)
        /// </summary>
        private static ConcurrentDictionary<string, object> ConfigObjectValues = new ConcurrentDictionary<string, object>();
        IDictOp DictOp;
        public ConfigOp(IDictOp dictOp)
        {
            DictOp = dictOp;
        }
        //获取分页数据
        public Core.DBContext.Page<ConfigGrid> GetPageList(int page, int rows, int group = 0, int opType = -1, string confName = null)
        {
            var sql = "SELECT a.*,b.dictname groupname from web_config a INNER JOIN sys_dict b on a.cgroup=b.dictid";
            if (group != 0)
                sql += " and a.cgroup=@0";
            if (opType != -1)
                sql += " and a.ctype=@1";
            if (!string.IsNullOrEmpty(confName))
                sql += " and a.keyname like @2";
            sql += " ORDER BY b.sortindex ASC,a.sortindex ASC";
            return Context.Instance.Page<ConfigGrid>(page, rows, sql, group, opType, "%" + confName + "%");
        }

        public List<ConfigSet> GetConfList()
        {
            var sql = "SELECT a.*,b.dictname groupname from web_config a INNER JOIN sys_dict b on a.cgroup=b.dictid ORDER BY b.sortindex ASC,a.sortindex ASC";
            return Context.Instance.Query<ConfigSet>(sql).ToList();
        }

        public IEnumerable<KeyValue> GetGroupList()
        {
            return DictOp.GetKeyValuesByCode("ConfType");
        }

        public ConfigInfo GetModelByKey(string key)
        {
            return Context.Instance.SingleOrDefault<ConfigInfo>(" where ckey=@0 LIMIT 1", key);
        }

        public bool KeyExits(string key)
        {
            return Context.Instance.ExecuteScalar<int>("SELECT count(configid) from web_config WHERE ckey=@0 LIMIT 1", key) > 0;
        }

        public bool SetValueByKey(int confId, string value)
        {
            return Context.Instance.Update<ConfigInfo>("set cvalue=@0 where configid=@1", value, confId) > 0;
        }

        public IEnumerable<KeyValue> GetListByGroupId(int groupid)
        {
            return Context.Instance.Query<ConfigInfo>("SELECT configid,keyname from web_config where cgroup=" + groupid).Select(o => new KeyValue { key = o.ConfigId.ToString(), value = o.KeyName });
        }

        public List<string> GetCheckConfigValueByKey(string key)
        {
            List<string> value = ConfigListValues.GetOrAdd(key, v =>
            {
                var config = GetModelByKey(v);
                return Context.Instance.Query<string>("SELECT a.values from web_confoption a WHERE array[" + config.CValue + "] @>array[a.optionid]").ToList();
            });
            return value;
        }

        public string GetRadioConfigValueByKey(string key)
        {
            var value = ConfigValues.GetOrAdd(key, v =>
            {
                var config = GetModelByKey(v);
                return Context.Instance.SingleOrDefault<string>("SELECT a.values from web_confoption a WHERE a.optionid=@0", config.CValue);
            });
            return value;
        }

        public string GetConfigValueByKey(string key)
        {
            return ConfigValues.GetOrAdd(key, v => { return GetModelByKey(v).CValue; });
        }

        public T GetConfigObjectByKey<T>(string key)
        {
            return (T)ConfigObjectValues.GetOrAdd(key, v =>
            {
                var conf = GetModelByKey(v);
                return JsonConvert.DeserializeObject<T>(conf.CValue);
            });
        }

        public bool ClearConfigCache(string key = null)
        {
            if (!string.IsNullOrEmpty(key))
            {
                List<string> list; string v; object ov;
                ConfigListValues.TryRemove(key, out list);
                ConfigValues.TryRemove(key, out v);
                ConfigObjectValues.TryRemove(key, out ov);
                return true;
            }
            else
            {
                ConfigListValues.Clear();
                ConfigValues.Clear();
                ConfigObjectValues.Clear();
                return true;
            }
        }

        public IModelCacheFac<ConfigInfo> CacheOp
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException("配置功能不需要缓存操作");
            }
        }

        public object Add(ConfigInfo entity)
        {
            return Context.Instance.Insert(entity);
        }

        public int Delete(object id)
        {
            var DataContext = Context.Instance;
            DataContext.BeginTransaction();
            int count = 0;
            try
            {
               count+=DataContext.Delete<ConfigOption>("where configid=@0",id);
               count += DataContext.Delete<ConfigInfo>(id);
               DataContext.CompleteTransaction();
            }
            catch
            {
                DataContext.AbortTransaction();
            }
            return count;
        }

        public int Update(ConfigInfo entity)
        {
            return Context.Instance.Update(entity);
        }

        public int Update(ConfigInfo entity, string[] columns)
        {
            return Context.Instance.Update(entity,columns);
        }

        public ConfigInfo GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<ConfigInfo>(id);
        }
    }
}
