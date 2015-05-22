using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
    [TableName("Sys_Organ")]
    [PrimaryKey("OrganId", false)]
    [ExplicitColumns]
    public class Organ
    {
        [Column]
        public int OrganId
        {
            get;
            set;
        }
        [Column]
        public string OrganName
        {
            get;
            set;
        }
        [Column]
        public int TypeId
        {
            get;
            set;
        }
        [Column]
        public int Level
        {
            get;
            set;
        }
        [Column]
        public int ParentId
        {
            get;
            set;
        }
        [Column]
        public int State
        {
            get;
            set;
        }
    }
}
