using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DBExtensions
{
    [TableName("Sys_UserInfo")]
    [PrimaryKey("UserId", true)]
    [ExplicitColumns]
    [ProtoContract]
   public class WebLoginModel
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
        public string LoginId
        {
            get;
            set;
        }
        /// <summary>
        /// 会员类别
        /// </summary>
        [Column]
        [ProtoMember(2)]
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
        /// 头像
        /// </summary>
        [Column]
        [ProtoMember(3)]
        public string HeadImg
        {
            get;
            set;
        }
        /// <summary>
        /// 等级
        /// </summary>
        [Column]
        [ProtoMember(4)]
        public int Level
        {
            get;
            set;
        }
        /// <summary>
        /// 昵称
        /// </summary>
        [Column]
        [ProtoMember(5)]
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
