using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class OrganGrid:Organ
    {
        /// <summary>
        /// 类型名称
        /// </summary>
        [ResultColumn]
        public string TypeName
        {
            get;
            set;
        }
        /// <summary>
        /// 等级名称
        /// </summary>
        [ResultColumn]
        public string LevelName
        {
            get;
            set;
        }
        /// <summary>
        /// 父节点名称
        /// </summary>
        [ResultColumn]
        public string ParentName
        {
            get;
            set;
        }
    }
}
