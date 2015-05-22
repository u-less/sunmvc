using Sun.Model.DB;
using System.Collections.Generic;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class ConfigSet : ConfigInfo
    {
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
        /// 选项
        /// </summary>
        [Ignore]
        public List<ConfigOption> Options
        {
            get;
            set;
        }
    }
}
