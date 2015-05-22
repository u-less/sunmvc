using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Ioc;
using Sun.Core.Logging;
using Sun.Core.DBContext;
using Plugin.SunLog.DB;

namespace Plugin.SunLog
{
    public class Loger : ILoger
    {
        public ILogOperate Operate
        {
            get;
            set;
        }
        public Loger(ILogOperate operate)
        {
            Operate = operate;
        }

        public void Log(LogInfo info)
        {
            Operate.Insert(info);
        }

        public void Info(Exception e)
        {
            throw new NotImplementedException();
        }

        public void Info(string title, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Info(string title, string message)
        {
            throw new NotImplementedException();
        }

        public void Debug(Exception e)
        {
            throw new NotImplementedException();
        }

        public void Debug(string title, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Debug(string title, string message)
        {
            throw new NotImplementedException();
        }

        public void Warning(Exception e)
        {
            throw new NotImplementedException();
        }

        public void Warning(string title, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Warning(string title, string message)
        {
            throw new NotImplementedException();
        }

        public void Error(Exception e)
        {
            throw new NotImplementedException();
        }

        public void Error(string title, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Error(string title, string message)
        {
            throw new NotImplementedException();
        }

        public void Fatal(Exception e)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string title, Exception e)
        {
            throw new NotImplementedException();
        }

        public void Fatal(string title, string message)
        {
            throw new NotImplementedException();
        }

        public LogInfo GetLogById(int id)
        {
            throw new NotImplementedException();
        }

        public void DeleteById(int id)
        {
            throw new NotImplementedException();
        }

        public void Clear(DateTime endTime, LogLevel? level = null, bool isHand = true)
        {
            throw new NotImplementedException();
        }

        public void Handle(int id)
        {
            throw new NotImplementedException();
        }

        public Page<LogInfo> Page(int rows, int page, LogLevel? level = null, string title = null, bool? isHand = null)
        {
            throw new NotImplementedException();
        }
    }
}
