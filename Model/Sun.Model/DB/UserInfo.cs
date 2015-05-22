using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
   //会员信息表
    [TableName("Sys_UserInfo")]
    [PrimaryKey("UserId", true)]
    [ExplicitColumns]
    public class UserInfo
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
        /// 昵称
        /// </summary>
        [Column]
        public string NickName
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
        /// 地区编码
        /// </summary>
        [Column]
        public string RegionId
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
        /// 用户类型
        /// </summary>
        [Column]
        public int UserType
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
        /// 手机号码
        /// </summary>
        [Column]
        public string PhoneNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 性别(true:男,false:女)
        /// </summary>
        [Column]
        public bool Sex
        {
            get;
            set;
        }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Column]
        public string Email
        {
            get;
            set;
        }
        /// <summary>
        /// 姓名
        /// </summary>
        [Column]
        public string UserName
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
        /// 粉丝数量
        /// </summary>
        [Column]
        public int Fans
        {
            get;
            set;
        }
        /// <summary>
        /// 关注数量
        /// </summary>
        [Column]
        public int Focus
        {
            get;
            set;
        }
        /// <summary>
        /// 注册时间
        /// </summary>
        [Column]
        public DateTime AddTime
        {
            get;
            set;
        }
        /// <summary>
        /// 最后登录时间
        /// </summary>
        [Column]
        public DateTime LastLoginTime
        {
            get;
            set;
        }
        /// <summary>
        /// 最后登录IP地址
        /// </summary>
        [Column]
        public string LastLoginIp
        {
            get;
            set;
        }
        /// <summary>
        /// 绑定邮箱
        /// </summary>
        [Column]
        public bool IsBindEmail
        {
            get;
            set;
        }
        /// <summary>
        /// 绑定手机
        /// </summary>
        [Column]
        public bool IsBindPhone
        {
            get;
            set;
        }
        /// <summary>
        /// 菜单类型[0:left,1:top]
        /// </summary>
        [Column]
        public Int16 MenuType
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