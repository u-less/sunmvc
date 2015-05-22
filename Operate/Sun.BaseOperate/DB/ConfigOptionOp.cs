using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
     [IocExport(typeof(IConfOptionOp), true)]
    public class ConfigOptionOp : IConfOptionOp
    {
        public Core.DBContext.Page<ConfigOptionGrid> GetPageList(int page, int rows, int groupId = 0, int configId = 0, string optionName = null)
        {
            var sql = "SELECT a.*,b.ckey,b.keyname,b.cgroup,b.ctype,b.lock,c.dictname groupname from web_confoption a INNER JOIN web_config b ON a.configid=b.configid INNER JOIN sys_dict c on b.cgroup=c.dictid";
            if (groupId != 0)
                sql += " and b.cgroup=@0";
            if (configId != 0)
                sql += " and a.configid=@1";
            if (!string.IsNullOrEmpty(optionName))
                sql += " and a.optionname like @2";
            return Context.Instance.Page<ConfigOptionGrid>(page, rows, sql, groupId, configId, optionName, "%" + optionName + "%");
        }

        public IEnumerable<ConfigOption> GetOptionList()
        {
            return Context.Instance.Query<ConfigOption>("SELECT * from web_confoption");
        }

        public Model.DB.ConfigOption GetOptionById(int optionId)
        {
            return Context.Instance.SingleOrDefault<ConfigOption>(optionId);
        }

        public ConfigOptionGrid GetOptionGridById(int optionId)
        {
            var sql = "SELECT a.*,b.ckey,b.keyname,b.cgroup,b.ctype,b.lock,c.dictname groupname from web_confoption a INNER JOIN web_config b ON a.configid=b.configid INNER JOIN sys_dict c on b.cgroup=c.dictid where optionid=@0";
            return Context.Instance.SingleOrDefault<ConfigOptionGrid>(sql, optionId);
        }

        public IModelCacheFac<Model.DB.ConfigOption> CacheOp
        {
            get
            {
                return null;
            }
            set
            {
                throw new NotImplementedException("该模块赞不支持缓存");
            }
        }

        public object Add(ConfigOption entity)
        {
            return Context.Instance.Insert(entity);
        }

        public int Delete(object id)
        {
            return Context.Instance.Delete<ConfigOption>(id);
        }

        public int Update(Model.DB.ConfigOption entity)
        {
            return Context.Instance.Update(entity);
        }

        public int Update(Model.DB.ConfigOption entity, string[] columns)
        {
            return Context.Instance.Update(entity,columns);
        }

        public ConfigOption GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<ConfigOption>(id);
        }
    }
}
