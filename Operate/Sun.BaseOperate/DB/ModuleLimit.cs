using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Framework.Login;
using Sun.Core.Ioc;
using Sun.BaseOperate.Interface;
using Autofac;

namespace Sun.BaseOperate.DB
{
    [IocExport(typeof(IModuleLimit), true)]
    public class ModuleLimit : IModuleLimit
    {
        public List<int> GetLimitsByKey(string moduleKey, string roleId)
        {
            return WebIoc.Container.Resolve<IRoleLimitOp>().GetLimitsByModuleAndRole(moduleKey, roleId);
        }

        public string GetModuleValue(string moduleKey)
        {
            return WebIoc.Container.Resolve<IModuleOp>().GetModuleByKey(moduleKey).ModuleValue;
        }
    }
}
