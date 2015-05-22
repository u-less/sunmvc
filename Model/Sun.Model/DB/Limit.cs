using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
    //权限信息表
    [TableName("Sys_Limit")]
    [PrimaryKey("LimitId", true)]
    [ExplicitColumns]
    public class Limit
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int LimitId
        {
            get;
            set;
        }
        /// <summary>
        /// 编码
        /// </summary>
        [Column]
        public int Code
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Column]
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 所属模块
        /// </summary>
        [Column]
        public int ModuleId
        {
            get;
            set;
        }
        /// <summary>
        /// 单选组
        /// </summary>
        [Column]
        public string RadioGroup
        {
            get;
            set;
        }
        /// <summary>
        /// 图标
        /// </summary>
        [Column]
        public string Icon
        {
            get;
            set;
        }
    }
}