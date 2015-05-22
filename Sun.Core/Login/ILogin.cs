using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Login
{
    public interface Ilogin
    {
        /// <summary>
        /// 验证码验证
        /// </summary>
        /// <param name="captcha">验证码</param>
        /// <param name="index">同页面验证码的序列</param>
        /// <returns></returns>
        bool CheckCaptcha(string captcha);
        /// <summary>
        /// 无验证码登录
        /// </summary>
        /// <param name="LoginId">账号</param>
        /// <param name="password">密码</param>
        /// <param name="type">登录类型</param>
        /// <returns>登录结果</returns>
        LoginState Login(string loginId,string password, bool remember);
        /// <summary>
        /// 含验证码登录
        /// </summary>
        /// <param name="LoginId">账号</param>
        /// <param name="password">密码</param>
        /// <param name="captcha">验证码</param>
        /// <param name="type">登录模式</param>
        /// <returns>登录结果</returns>
        LoginState Login(string loginId, string password, string captcha, bool remember);
        /// <summary>
        /// 移出登录信息
        /// </summary>
        void RemoveLoginInfo();
        /// <summary>
        /// 获取当前登录信息
        /// </summary>
        /// <returns>登录保存信息</returns>
        LoginInfo GetLoginInfo();
        /// <summary>
        /// 获取用户操作失败次数
        /// </summary>
        /// <returns></returns>
        int GetErrors();
    }
}
