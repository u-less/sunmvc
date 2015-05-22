using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class BannerGrid : Banner
    {
        //类型名称
        [ResultColumn]
        public string TypeName
        {
            get;
            set;
        }

        //布局名称
        [ResultColumn]
        public string LayOutName
        {
            get;
            set;
        }

        //关联名称
        [ResultColumn]
        public string RelateName
        {
            get;
            set;
        }
    }
}
