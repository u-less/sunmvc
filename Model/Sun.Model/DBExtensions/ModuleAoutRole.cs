using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class ModuleAoutRole:Module
    {
        [ResultColumn]
        public int HasLimit
        {
            get;
            set;
        }
    }
}
