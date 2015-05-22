using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Sun.Framework.Session
{
    public class CookieHelper
    {
        /// <summary>
        /// 添加Cookie
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Add(string key, string value)
        {
            HttpContext.Current.Response.AppendCookie(new HttpCookie(key, value));
        }
        public static void Add(string key, string value, DateTime expireTime)
        {
            HttpCookie cookie = new HttpCookie(key, value);
            cookie.Expires = expireTime;
            HttpContext.Current.Response.AppendCookie(cookie);
        }
        /// <summary>
        /// 读取Cookie
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool TryGetValue(string key, out string value)
        {
            if (HttpContext.Current.Request.Cookies[key] == null)
            {
                value = null;
                return false;
            };
            value = HttpContext.Current.Request.Cookies[HttpUtility.UrlEncode(key)].Value;
            if (!string.IsNullOrEmpty(value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 移除Cookie
        /// </summary>
        /// <param name="key"></param>
        public static void Remove(string key)
        {
            HttpContext.Current.Response.Cookies.Remove(key);
            HttpContext.Current.Response.Cookies[key].Expires = System.DateTime.Now.AddDays(-1);
        }
    }
}
