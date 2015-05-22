using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class ConfigGrid : ConfigInfo
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
    }
}
