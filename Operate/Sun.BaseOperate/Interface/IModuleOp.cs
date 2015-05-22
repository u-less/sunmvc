using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;
using Sun.Model.DB;
using Sun.Model.Common;

namespace Sun.BaseOperate.Interface
{
    public interface IModuleOp : IDBOperate<Module>
    {
        bool KeyExits(string key, int id = 0);
        string GetModuleGridJson();
        string GetModuleComboTreeJson();
        string GetModuleNoRootComboTreeJson();
        IEnumerable<Module> GetRoleModule(int roleId);
        string GetRoleModuleJson(int roleId);
        SEOInfo GetSEOInfoByKey(string modeuleKey);
        List<Module> GetListByParentKey(string key);

        Module GetModuleByKey(string key);
        IEnumerable<KeyValue> GetModuelTypeCombo();
    }
}
