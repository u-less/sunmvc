using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Ioc;
using Sun.Core.Logging;
using Sun.Core.DBContext;

namespace Plugin.SunLog.DB
{
    [IocExport(typeof(ILogOperate))]
    public class LogOperate:ILogOperate
    {
        string createTable="create table loginfo(id SERIAL not null,ipaddress VARCHAR(80) null,level INT2 null,title VARCHAR(500) null,message text null,logtime TIMESTAMP null,status BOOL null,constraint PK_LOG primary key (id));";
        public bool Init()
        {
            if (DBContext.Instance.ExecuteScalar<int>("select count(*) from pg_class where relname ='loginfo'") == 0)
            {
                DBContext.Instance.Execute(string.Format(createTable, LogLevel.Info.ToString()));
            }
            return true;
        }

        public int Insert(Sun.Core.Logging.LogInfo info)
        {
            var sql = "INSERT INTO loginfo(ipaddress,level,title,message,logtime,status)values(@0,@1,@2,@3,@4,@5)";
            return DBContext.Instance.Execute(new Sql(sql, info.IPAddress, info.Level, info.Title, info.Message, info.LogTime, info.Status));
        }

        public int Delete(int id)
        {
            return DBContext.Instance.Execute("delete from loginfo where id=@0",id);
        }

        public int Clear(DateTime endTime, Sun.Core.Logging.LogLevel? level = null, bool status = true)
        {
            throw new NotImplementedException();
        }

        public int SetStatus(int id, bool status)
        {
            throw new NotImplementedException();
        }

        public Sun.Core.DBContext.Page<Sun.Core.Logging.LogInfo> Page(int rows, int page, Sun.Core.Logging.LogLevel? level = null, string title = null, bool? status = null)
        {
            throw new NotImplementedException();
        }
    }
}
