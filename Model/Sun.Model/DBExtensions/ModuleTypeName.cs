using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class ModuleTypeName : Module
    {
        [ResultColumn]
        public string TypeName
        {
            get;
            set;
        }
    }
}
