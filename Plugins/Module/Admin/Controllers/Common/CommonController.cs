using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Sun.Model.DBExtensions;
using Sun.BaseOperate.Interface;
using Sun.Framework.SunImg;
using Sun.BaseOperate.Config;
using Sun.Framework.Calculate;
using Sun.Core.Ioc;
using Sun.Core.Session;
using Sun.Core.Message;
using Sun.Core.Login;
using Autofac;


namespace Plugin.Admin.Controllers.Web
{
    public class CommonController  : Controller
    {
        ISession Session;
        public CommonController(ISession session)
        {
            this.Session = session;
        }
        /// <summary>
        /// 获取验证码图片
        /// </summary>
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetCaptchaImage()
        {
            var captcha = CaptchaCode.DrawNumberImage(4);
            Session.Set(CustomConfig.CaptchaSession, captcha.Result);
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ContentType = "image/gif";
            System.Web.HttpContext.Current.Response.BinaryWrite(captcha.ImgData);
            System.Web.HttpContext.Current.Response.End();
        }
        //获取九宫格验证码
        [AcceptVerbs(HttpVerbs.Get)]
        public void GetSudokuCaptchaImage()
        {
            var cap = CaptchaCode.DrawChinaSudokuImage();
            byte[] CaptchaData = cap.ImgData;
            Session.Set(CustomConfig.CaptchaSession, cap.Result);
            System.Web.HttpContext.Current.Response.ClearContent();
            System.Web.HttpContext.Current.Response.ContentType = "image/gif";
            System.Web.HttpContext.Current.Response.BinaryWrite(CaptchaData);
            System.Web.HttpContext.Current.Response.End();
        }
        //对比九宫格验证码
        public JsonResult CompareSudokuCaptcha(string captcha)
        {
            var right = Session[CustomConfig.CaptchaSession].ToString();
            return Json(right == captcha);
        }
        //短信验证码
        [HttpPost]
        public JsonResult GetSMSCaptcha(string mobile)
        {
            var captcha =RandomHelper.Number(6);
            Session.Set(CustomConfig.CaptchaSession, captcha);
            ISMS sms = WebIoc.Container.Resolve<ISMS>();
            var result = sms.sendOnce(mobile, string.Format(CustomConfig.PhoneCapTemp, captcha));
            return Json(result);
        }

        //网站用户登录状态异步Ajax获取
        public JsonResult WebLoginInfo()
        {
            var loginInfo = LoginFac.Admin.GetLoginInfo();
            if (loginInfo.UserId!=0)
            {
                return Json(new { UserId = loginInfo.UserId, UserType = loginInfo.Type, Level = loginInfo.Level, NickName = loginInfo.NickName, HeadImg = loginInfo.HeadImg });
            }
            else
                return Json(null);
        }
	}
}