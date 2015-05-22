using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Logging
{
    public class LogInfo
    {
        /// <summary>
        /// 日志编号
        /// </summary>
        public int Id
        {
            get;
            set;
        }
        /// <summary>
        /// 日志来源位置介绍
        /// </summary>
        public string IPAddress
        {
            get;
            set;
        }
        
        /// <summary>
        /// 日志级别
        /// </summary>
        public LogLevel Level
        {
            get;
            set;
        }
        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 日志详细信息
        /// </summary>
        public string Message
        {
            get;
            set;
        }
        /// <summary>
        /// 日志添加时间
        /// </summary>
        public DateTime LogTime
        {
            get;
            set;
        }
        /// <summary>
        /// 日志状态(未处理或已处理)
        /// </summary>
        public bool Status
        {
            get;
            set;
        }
    }
}
