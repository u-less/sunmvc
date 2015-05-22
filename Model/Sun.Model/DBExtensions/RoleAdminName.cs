using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class RoleGrid:Role
    {
        [ResultColumn]
        public string AdminName
        {
            get;
            set;
        }
        [ResultColumn]
        public string OrganName
        {
            get;
            set;
        }
    }
}
