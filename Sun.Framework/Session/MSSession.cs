using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Session;

namespace Sun.Framework.Session
{
    public class MSSession : ISession
    {
        public MSSession()
        {
            System.Web.HttpContext.Current.Session.Timeout = 20;
        }
        public string this[string key]
        {
            get
            {
                string result = null;
                var v = System.Web.HttpContext.Current.Session[key];
                if (v != null)
                    result = v.ToString();
                return result;
            }
            set
            {
                System.Web.HttpContext.Current.Session[key] = value;
            }
        }

        public void Set(string key, string value, TimeSpan? expire = null)
        {
            System.Web.HttpContext.Current.Session[key] = value;
        }

        public string Get(string key)
        {
            string result = null;
            var v = System.Web.HttpContext.Current.Session[key];
            if (v != null)
                result = v.ToString();
            return result;
        }

        public void Delete(string key)
        {
            System.Web.HttpContext.Current.Session[key] = null;
        }


        public void Set<T>(string key, T value, TimeSpan? expire = null) where T : class
        {
            System.Web.HttpContext.Current.Session[key] = value;
        }

        public T Get<T>(string key) where T : class
        {
            T result = null;
            var v = System.Web.HttpContext.Current.Session[key];
            if (v != null)
                result = (T)v;
            return result;
        }
    }
}
