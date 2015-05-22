using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
    //网站配置表
    [TableName("Web_Config")]
    [PrimaryKey("ConfigId", true)]
    [ExplicitColumns]
    public class ConfigInfo
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int ConfigId
        {
            get;
            set;
        }
        /// <summary>
        /// 配置项类型(单选、多选...)
        /// </summary>
        [Column]
        public int CType
        {
            get;
            set;
        }
        /// <summary>
        /// 配置组
        /// </summary>
        [Column]
        public int CGroup
        {
            get;
            set;
        }
        /// <summary>
        /// 标识
        /// </summary>
        [Column]
        public string CKey
        {
            get;
            set;
        }
        /// <summary>
        /// 标识名称
        /// </summary>
        [Column]
        public string KeyName
        {
            get;
            set;
        }
        /// <summary>
        /// 内容
        /// </summary>
        [Column]
        public string CValue
        {
            get;
            set;
        }
        /// <summary>
        /// 验证类型
        /// </summary>
        [Column]
        public string ValidType
        {
            get;
            set;
        }
        /// <summary>
        /// 规则描述
        /// </summary>
        [Column]
        public string CRule
        {
            get;
            set;
        }
        /// <summary>
        /// 排序顺序
        /// </summary>
        [Column]
        public int SortIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 锁定标识
        /// </summary>
        [Column]
        public bool Lock
        {
            get;
            set;
        }
    }
}