using System;
using Sun.Model.DB;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DBExtensions
{
    /// <summary>
    /// 会员中心顶部会员信息获取
    /// </summary>
    [TableName("Sys_UserInfo")]
    [PrimaryKey("UserId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class UserHeadInfo
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
        /// 地区编号
        /// </summary>
        [Column]
        public int RegionId
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
        /// 性别(true:男,false:女)
        /// </summary>
        [Column]
        public bool Sex
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
        /// 签名
        /// </summary>
        [Column]
        public string Sign
        {
            get;
            set;
        }
    }
}
