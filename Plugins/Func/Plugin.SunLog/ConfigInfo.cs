using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.SunLog
{
    public class ConfigInfo
    {
        /// <summary>
        /// 数据库链接名(对应web.config)
        /// </summary>
        public string ConnectionName
        {
            get;
            set;
        }
        /// <summary>
        /// 需要邮件通知的日志级别
        /// </summary>
        public string EmailLevel
        {
            get;
            set;
        }
    }
}
