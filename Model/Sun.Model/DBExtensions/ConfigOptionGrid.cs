using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class ConfigOptionGrid : ConfigOption
    {
        /// <summary>
        /// 配置项名称
        /// </summary>
        [ResultColumn]
        public string ConfName
        {
            get;
            set;
        }

        /// <summary>
        /// 所属分组Id
        /// </summary>
        [ResultColumn]
        public int CGroup
        {
            get;
            set;
        }

        /// <summary>
        /// 所属分组名称
        /// </summary>
        [ResultColumn]
        public string GroupName
        {
            get;
            set;
        }

        /// <summary>
        /// 配置项名称
        /// </summary>
        [ResultColumn]
        public string KeyName
        {
            get;
            set;
        }
    }
}
