using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Ioc;
using Autofac;
using Autofac.Features.Indexed;

namespace Sun.Core.Login
{
    public class LoginFac
    {
        static Ilogin admin, web;
        public static Ilogin Admin
        {
            get
            {
                if (null == admin)
                    admin = WebIoc.Container.Resolve<IIndex<LoginType, Ilogin>>()[LoginType.Admin];
                return admin;
            }
        }
        public static Ilogin Web
        {
            get
            {
                if(null==web)
                    web = WebIoc.Container.Resolve<IIndex<LoginType, Ilogin>>()[LoginType.Web];
                return web;
            }
        }
    }
}
