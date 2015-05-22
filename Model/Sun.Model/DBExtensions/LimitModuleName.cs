using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class LimitModuleName:Limit
    {
        [ResultColumn]
        public string ModuleName
        {
            get;
            set;
        }
    }
}
