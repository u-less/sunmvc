using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DB
{
    /// <summary>
    /// 用户积分信息表
    /// </summary>
    [TableName("UserPoint")]
    [PrimaryKey("UserId", false)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class UserPoint
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        [Column]
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 问答积分
        /// </summary>
        [Column]
        public int Answer
        {
            get;
            set;
        }
        /// <summary>
        /// 消费积分
        /// </summary>
        [Column]
        public int Expense
        {
            get;
            set;
        }
        /// <summary>
        /// 活跃积分
        /// </summary>
        [Column]
        public int Active
        {
            get;
            set;
        }
        /// <summary>
        /// 文章贡献积分
        /// </summary>
        [Column]
        public int Article
        {
            get;
            set;
        }
        /// <summary>
        /// 消费的问答积分
        /// </summary>
        [Column]
        public int SubtractAnswer
        {
            get;
            set;
        }
        /// <summary>
        /// 消费的消费积分
        /// </summary>
        [Column]
        public int SubtractExpense
        {
            get;
            set;
        }
        /// <summary>
        /// 消费的活跃积分
        /// </summary>
        [Column]
        public int SubtractActive
        {
            get;
            set;
        }
        /// <summary>
        /// 消费的文章贡献积分
        /// </summary>
        [Column]
        public int SubtractArticle
        {
            get;
            set;
        }
    }
}
