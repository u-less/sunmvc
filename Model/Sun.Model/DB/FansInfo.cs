using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DB
{
    [TableName("FansInfo")]
    [PrimaryKey("FansId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class FansInfo
    {
        /// <summary>
        /// 粉丝编号
        /// </summary>
        [Column]
        public int FansId
        {
            get;
            set;
        }
        /// <summary>
        /// 关注人
        /// </summary>
        [Column]
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 被关注人
        /// </summary>
        [Column]
        public int ToUserId
        {
            get;
            set;
        }
        /// <summary>
        /// 相关昵称
        /// </summary>
        [ResultColumn]
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 相关头像
        /// </summary>
        [ResultColumn]
        public string HeadImg
        {
            get;
            set;
        }
        /// <summary>
        /// userId是否被toUserId关注
        /// </summary>
        [ResultColumn]
        public bool Follow
        {
            get;
            set;
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        [Column]
        public DateTime AddTime
        {
            get;
            set;
        }
    }
}
