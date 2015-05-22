using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sun.BaseOperate.Config
{
    public partial class CustomConfig
    {
        /// <summary>
        /// 网站名称
        /// </summary>
        public static string WebName
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("WebName");
            }
        }
        /// <summary>
        /// 网站主域名
        /// </summary>
        public static string WebDomain
        {
            get {
                return ConfigOp.GetConfigValueByKey("WebDomain");
            }
        }
        /// <summary>
        /// 允许不使用验证码登录的错误次数
        /// </summary>
        public static int loginCanErrors
        {
            get
            {
                return int.Parse(ConfigOp.GetConfigValueByKey("LoginCanErrors"));
            }
        }
        /// <summary>
        /// 后台logo图片地址
        /// </summary>
        public static string LogoUrl
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("LogoUrl");
            }
        }
        /// <summary>
        /// 网站Logo
        /// </summary>
        public static string WebLogoUrl
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("WebLogo");
            }
        }
        /// <summary>
        /// 找回密码邮箱内容模板
        /// </summary>
        public static string FindPwdEmailTemp
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("FindPwdEmail");
            }
        }
        /// <summary>
        /// 绑定邮箱模板
        /// </summary>
        public static string BindEmailTemp
        {
            get {
                return ConfigOp.GetConfigValueByKey("BindEmailTemp");
            }
        }
        /// <summary>
        /// 修改邮件
        /// </summary>
        public static string UpdateEmailTemp
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("UpdateBindEmail");
            }
        }
        /// <summary>
        /// 手机验证码发送模板
        /// </summary>
        public static string PhoneCapTemp
        {
            get {
                return ConfigOp.GetConfigValueByKey("PhoneCapTemp");
            }
        }
        private static DateTime _DefaultTime = new DateTime(2014, 6, 8, 08, 08, 08);
        /// <summary>
        /// 系统默认时间
        /// </summary>
        public static DateTime DefaultTime
        {
            get
            {
                return _DefaultTime;
            }
        }
        /// <summary>
        /// 系统错误通知对象信息
        /// </summary>
        public static ErrorNotification ErrorNotification
        {
            get {
                return ConfigOp.GetConfigObjectByKey<ErrorNotification>("ErrorNotification");
            }
        }
        /// <summary>
        /// 超级管理员角色编号
        /// </summary>
        public static int SuperRoleId
        {
            get {
                return int.Parse(System.Configuration.ConfigurationManager.AppSettings["SuperRoleId"]);
            }
        }
    }
    public class ErrorNotification
    {
        /// <summary>
        /// 邮箱列表
        /// </summary>
        public string[] Email
        {
            get;
            set;
        }
        /// <summary>
        /// 手机列表
        /// </summary>
        public string[] Mobile
        {
            get;
            set;
        }
    }
}