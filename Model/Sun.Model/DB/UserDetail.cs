using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
    //会员详细信息表
    [TableName("Sys_UserDetail")]
    [PrimaryKey("UserId", false)]
    [ExplicitColumns]
    public class UserDetail
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
        /// 身份证
        /// </summary>
        [Column]
        public string IdentityId
        {
            get;
            set;
        }
        /// <summary>
        /// 口令
        /// </summary>
        [Column]
        public string Command
        {
            get;
            set;
        }
        /// <summary>
        /// 密保问题一
        /// </summary>
        [Column]
        public string OnePassProblem
        {
            get;
            set;
        }
        /// <summary>
        /// 密保答案一
        /// </summary>
        [Column]
        public string OnePassAnswer
        {
            get;
            set;
        }
        /// <summary>
        /// 密保问题二
        /// </summary>
        [Column]
        public string TwoPassProblem
        {
            get;
            set;
        }
        /// <summary>
        /// 密保答案二
        /// </summary>
        [Column]
        public string TwoPassAnswer
        {
            get;
            set;
        }
        /// <summary>
        /// 固定电话
        /// </summary>
        [Column]
        public string CallNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 生日
        /// </summary>
        [Column]
        public DateTime Birthday
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
        /// <summary>
        /// 喜欢的旅游类型
        /// </summary>
        [Column]
        public string LikeTravelType
        {
            get;
            set;
        }
        /// <summary>
        /// QQ
        /// </summary>
        [Column]
        public string QQ
        {
            get;
            set;
        }
        /// <summary>
        /// 地址
        /// </summary>
        [Column]
        public string Address
        {
            get;
            set;
        }
    }
}