using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Login;
using Sun.Core.Ioc;
using Sun.Core.Session;
using Sun.Framework.Security;
using Sun.BaseOperate.Config;
using Sun.BaseOperate.DbContext;
using Sun.Model.Common;

namespace Sun.BaseOperate.Login
{
    [IocExport(typeof(Ilogin), ContractKey = LoginType.Admin, SingleInstance = true)]
    public class AdminLogin : Ilogin
    {
        internal const string adminUserSession = "admin_user";
        ISession Session;
        public AdminLogin(ISession session)
        {
            Session = session;
        }
        public bool CheckCaptcha(string captcha)
        {
            var result = Session[CustomConfig.CaptchaSession] == captcha.ToLower();
            if (result)
                Session.Delete(CustomConfig.CaptchaSession);
            return result;
        }

        public LoginState Login(string loginId, string password, bool remember)
        {
            LoginUser user = null;
            password = Md5Encrypt.PasswordEncode(password);
            user =Context.Instance.SingleOrDefault<LoginUser>("SELECT a.userid,a.usertype,a.password,a.roleid,a.level,a.organids,a.nickname,a.states from sys_userinfo a where (a.loginid=@0 or (a.isbindemail='true' AND a.email=@0) or (a.isbindphone='true' AND a.phonenumber=@0)) and a.usertype=@1", loginId, (int)UserType.Admin);
            if (user != null && user.UserId != 0)
            {
                if (user.Password != password)
                {
                    SetErrors();
                    return LoginState.PasswordError;
                }
                if ((UserStatus)user.States == UserStatus.Lock)
                    return LoginState.IsLock;
                var loginInfo = new LoginInfo() { UserId=user.UserId,Type=user.UserType,Level=user.Level, HeadImg=user.HeadImg, NickName=user.NickName, OrganId=user.OrganIds, RoleId=user.RoleId};
                if (!remember)
                    SaveLoginInfo(loginInfo);
                else
                    SaveLoginInfo(loginInfo,new TimeSpan(100, 0, 0));
                return LoginState.Success;
            }
            else
            {
                SetErrors();
                return LoginState.NoLoginId;
            }
        }
        /// <summary>
        /// 设置登录错误的次数
        /// </summary>
        /// <returns></returns>
        private void SetErrors()
        {
            var errors = Session["errors"];
            var count = errors != null ? Convert.ToInt32(errors) : 0;
            if (count < CustomConfig.loginCanErrors)
            {
                var newcount = count + 1;
                Session["errors"] = newcount.ToString();
            }
        }
        /// <summary>
        /// 获取登陆错误的次数
        /// </summary>
        /// <returns></returns>
        public int GetErrors()
        {
            var errors = Session["errors"];
            return errors != null ? Convert.ToInt32(errors) : 0;
        }
        public LoginState Login(string loginId, string password, string captcha, bool remember)
        {
            if (GetErrors() < CustomConfig.loginCanErrors)
                return Login(loginId, password, remember);
            else
            {
                if (!CheckCaptcha(captcha))
                    return LoginState.CaptchaError;
                else
                    return Login(loginId, password, remember);
            }
        }

        public void RemoveLoginInfo()
        {
            Session.Delete(adminUserSession);
        }

        public LoginInfo GetLoginInfo()
        {
            return Session.Get<LoginInfo>(adminUserSession) ?? new LoginInfo();
        }
        /// <summary>
        /// 保存登陆信息
        /// </summary>
        /// <param name="info"></param>
        /// <param name="expire">过期时间</param>
        public void SaveLoginInfo(LoginInfo info, TimeSpan? expire = null)
        {
            Session.Set<LoginInfo>(adminUserSession, info, expire);
        }
    }
}
