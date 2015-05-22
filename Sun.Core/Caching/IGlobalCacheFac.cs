using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Caching
{
    public interface IGlobalCacheFac
    {
        void UpdateByKey(string key);
        void UpdateComplate(string key);
        void UpdateAll(DateTime? updateTime = null);
        IEnumerable<string> GetNeedUpdateKeyList();
    }
}
