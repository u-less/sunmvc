using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Plugin;
using Sun.Core.Ioc;

namespace Plugin.MongoUpload
{
    [IocExport(typeof(IPlugin), ContractName = "f6fc61b5-2314-4149-aebb-3de0eb83221e", SingleInstance = true)]
    public class MongoUploadPlugin:IPlugin
    {
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public PluginInfo Info
        {
            get { throw new NotImplementedException(); }
        }

        public void Initialize(PluginInfo info)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
