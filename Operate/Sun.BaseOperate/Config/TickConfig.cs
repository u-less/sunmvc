using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.BaseOperate.Config
{
    public partial class CustomConfig
    {
        /// <summary>
        /// 绑定邮箱邮件ticks
        /// </summary>
        public const int BindEmailTicks = 10;
        /// <summary>
        /// 修改绑定邮箱邮件ticks
        /// </summary>
        public const int UpdateEmailTikcs = 11;
        /// <summary>
        /// 找回密码邮件ticks
        /// </summary>
        public const int FindPwdEmailTicks = 12;
        /// <summary>
        /// 图片验证码Session
        /// </summary>
        public const string CaptchaSession = "ics";
    }
}
