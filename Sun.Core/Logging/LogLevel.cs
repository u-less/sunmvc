using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Logging
{
    public enum LogLevel
    { 
        /// <summary>
        /// 信息
        /// </summary>
        Info=0,
        /// <summary>
        /// 提醒
        /// </summary>
        Warning=1,
        /// <summary>
        /// BUG
        /// </summary>
        Debug =2,
        /// <summary>
        /// 错误
        /// </summary>
        Error=3,
        /// <summary>
        /// 致命问题
        /// </summary>
        Fatal=4
    }
}
