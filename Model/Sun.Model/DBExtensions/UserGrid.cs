using System;
using Sun.Model.DB;
using Sun.Core.DBContext;

namespace Sun.Model.DBExtensions
{
    public class UserGrid : UserInfo
    {
        /// <summary>
        /// 机构名称
        /// </summary>
        [ResultColumn]
        public string OrganName
        {
            get;
            set;
        }
        /// <summary>
        /// 角色
        /// </summary>
        [ResultColumn]
        public string RoleName
        {
            get;
            set;
        }
        /// <summary>
        /// 地区
        /// </summary>
        [ResultColumn]
        public string RegionName
        {
            get;
            set;
        }
        //身份证号
        [ResultColumn]
        public string IdentityId
        {
            get;
            set;
        }

        //固定电话
        [ResultColumn]
        public string CallNumber
        {
            get;
            set;
        }

        //生日
        [ResultColumn]
        public DateTime Birthday
        {
            get;
            set;
        }

        /// <summary>
        /// 头像
        /// </summary>
        [ResultColumn]
        public string HeadPictrue
        {
            get;
            set;
        }

        //qq
        [ResultColumn]
        public string QQ
        {
            get;
            set;
        }

        //地址
        [ResultColumn]
        public string Address
        {
            get;
            set;
        }
    }
}
