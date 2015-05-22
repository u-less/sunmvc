using System;
using System.ComponentModel;

namespace Sun.Model.Common
{
    public enum UserType
    {
        /// <summary>
        /// 会员
        /// </summary>
        [Description("会员")]
        Vip = 0,
        /// <summary>
        /// 商家
        /// </summary>
        [Description("商家")]
        Shops = 1,
        /// <summary>
        /// 管理员
        /// </summary>
        [Description("管理员")]
        Admin = 2
    }
}
