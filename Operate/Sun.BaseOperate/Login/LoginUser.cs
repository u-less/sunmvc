using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;

namespace Sun.BaseOperate.Login
{
    [TableName("Sys_UserInfo")]
    [PrimaryKey("UserId", true)]
    [ExplicitColumns]
    public class LoginUser
    {
        /// <summary>
        /// 会员编号
        /// </summary>
        [Column]
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 会员账号
        /// </summary>
        [Column]
        public string LoginId
        {
            get;
            set;
        }
        /// <summary>
        /// 会员类别
        /// </summary>
        [Column]
        public int UserType
        {
            get;
            set;
        }
        /// <summary>
        /// 密码
        /// </summary>
        [Column]
        public string Password
        {
            get;
            set;
        }
        /// <summary>
        /// 机构
        /// </summary>
        [Column]
        public string OrganIds
        {
            get;
            set;
        }
        /// <summary>
        /// 头像
        /// </summary>
        [Column]
        public string HeadImg
        {
            get;
            set;
        }
        /// <summary>
        /// 所属角色
        /// </summary>
        [Column]
        public int RoleId
        {
            get;
            set;
        }
        /// <summary>
        /// 等级
        /// </summary>
        [Column]
        public int Level
        {
            get;
            set;
        }
        /// <summary>
        /// 昵称
        /// </summary>
        [Column]
        public string NickName
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        [Column]
        public int States
        {
            get;
            set;
        }
    }
}
