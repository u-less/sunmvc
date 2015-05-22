using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DBExtensions
{
    [TableName("Sys_UserInfo")]
    [PrimaryKey("UserId", true)]
    [ExplicitColumns]
    [ProtoContract]
    public class AdminLoginModel
    {
        /// <summary>
        /// 会员编号
        /// </summary>
        [Column]
        [ProtoMember(1)]
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 会员账号
        /// </summary>
        [Column]
        [ProtoMember(2)]
        public string LoginId
        {
            get;
            set;
        }
        /// <summary>
        /// 会员类别
        /// </summary>
        [Column]
        [ProtoMember(3)]
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
        [ProtoMember(4)]
        public string OrganIds
        {
            get;
            set;
        }
        /// <summary>
        /// 机构名称
        /// </summary>
        [ResultColumn]
        [ProtoMember(5)]
        public string OrganNames
        {
            get;set;
        }
        /// <summary>
        /// 头像
        /// </summary>
        [Column]
        [ProtoMember(6)]
        public string HeadImg
        {
            get;
            set;
        }
        /// <summary>
        /// 所属角色
        /// </summary>
        [Column]
        [ProtoMember(7)]
        public int RoleId
        {
            get;
            set;
        }
        /// <summary>
        /// 等级
        /// </summary>
        [Column]
        [ProtoMember(8)]
        public int Level
        {
            get;
            set;
        }
        /// <summary>
        /// 昵称
        /// </summary>
        [Column]
        [ProtoMember(9)]
        public string NickName
        {
            get;
            set;
        }
        /// <summary>
        /// 用户菜单类型
        /// </summary>
        [Column]
        [ProtoMember(10)]
        public Int16 MenuType
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        [Column]
        [ProtoMember(11)]
        public int States
        {
            get;
            set;
        }
    }
}
