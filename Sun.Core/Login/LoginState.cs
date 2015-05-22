using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Login
{
    /// <summary>
    /// 登陆结果枚举
    /// </summary>
    public enum LoginState
    {
        /// <summary>
        /// 账号不存在
        /// </summary>
        NoLoginId = 0,
        /// <summary>
        /// 密码错误
        /// </summary>
        PasswordError = 1,
        /// <summary>
        /// 验证码输入错误
        /// </summary>
        CaptchaError = 2,
        /// <summary>
        /// 账号被锁定
        /// </summary>
        IsLock = 3,
        /// <summary>
        /// 登陆成功
        /// </summary>
        Success = 4,
        /// <summary>
        /// 账号异常
        /// </summary>
        Exception = 5,
        /// <summary>
        /// 用户类型错误
        /// </summary>
        TypeError = 6
    }
}
