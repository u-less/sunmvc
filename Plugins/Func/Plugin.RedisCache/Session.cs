using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Sun.Core.Caching;
using Sun.Core.Session;
using Sun.Framework.Session;

namespace Plugin.RedisCache
{
    public class Session:ISession
    {
        private static TimeSpan ExpireTimeSpan = new TimeSpan(1, 0, 0);
        ICache Cache;
        public Session(ICache cache)
        {
            Cache = cache;
        }
        public string this[string key]
        {
            get
            {
                return Get(key);
            }
            set
            {
                Set(key, value);
            } 
        }

        public void Set(string key, string value, TimeSpan? expire = null)
        {
            string sessionID;
            if (!CookieHelper.TryGetValue("SessionID", out sessionID))
            {
                sessionID = System.Web.HttpContext.Current.Session.SessionID;
                CookieHelper.Add("SessionID", sessionID);
            }
            Cache.StringSet(sessionID + "_" + key, value, expire ?? expire);
        }

        public void Set<T>(string key, T value, TimeSpan? expire = null) where T : class
        {
            string sessionID;
            if (!CookieHelper.TryGetValue("SessionID", out sessionID))
            {
                sessionID = System.Web.HttpContext.Current.Session.SessionID;
                CookieHelper.Add("SessionID", sessionID);
            }
            Cache.Set<T>(sessionID + "_" + key, value, expire ?? expire);
        }

        public string Get(string key)
        {
            string value = null;
            if (CookieHelper.TryGetValue("SessionID", out value))
            {
                Cache.Expire(value + "_" + key, ExpireTimeSpan);
                Cache.StringGet(value + "_" + key, out value);
            }
            return value;
        }

        public T Get<T>(string key) where T : class
        {
            string sessionId;
            T value = null;
            if (CookieHelper.TryGetValue("SessionID", out sessionId))
            {
                Cache.Expire(sessionId + "_" + key, ExpireTimeSpan);
                Cache.Get<T>(sessionId + "_" + key, out value);
            }
            return value;
        }

        public void Delete(string key)
        {
            string value = null;
            if (CookieHelper.TryGetValue("SessionID", out value))
            {
                Cache.Delete(value + "_" + key);
            }
        }
    }
}
