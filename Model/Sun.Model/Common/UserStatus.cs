using System;
using System.ComponentModel;

namespace Sun.Model.Common
{
    public enum UserStatus
    {
        /// <summary>
        /// 锁定
        /// </summary>
        [Description("锁定")]
        Lock = 0,
        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,
        /// <summary>
        /// 异常
        /// </summary>
        [Description("异常")]
        Exception = 2
    }
}
