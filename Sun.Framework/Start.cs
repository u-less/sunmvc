using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Ioc;
using Sun.Core.Plugin;
using Sun.Core.Session;
using Sun.Core.Message;
using Autofac;
using Sun.Framework.Plugin;
using Sun.Core.Caching;
using Sun.Framework.Message;
using Sun.Framework.Caching;

namespace Sun.Framework
{
    public class Start
    {
        public static void Init()
        {
            WebIoc.Register(b=>b.RegisterType<Session.MSSession>().As<ISession>().SingleInstance());
            WebIoc.Register(b => b.RegisterGeneric(typeof(ModelCacheFac<>)).As(typeof(IModelCacheFac<>)).SingleInstance());
            WebIoc.Register(b=>b.RegisterInstance(FileOperate.Instance).As<IFileOperate>().ExternallyOwned());
            WebIoc.Register(b=>b.RegisterInstance(Loader.Instance).As<ILoader>().ExternallyOwned());
            WebIoc.Register(b=>b.RegisterInstance(Manage.Instance).As<IManage>().ExternallyOwned());
            WebIoc.Register(b => b.RegisterInstance(ZipManage.Instance).As<IZipManage>().ExternallyOwned());
            WebIoc.Register(b=>b.RegisterType<SunSMS>().As<ISMS>().SingleInstance());
            WebIoc.Register(b => b.RegisterType<SunEmail>().As<IEmail>());
            ZipManage.Instance.Initialize();
            Manage.Instance.Initialize();
        }
    }
}
