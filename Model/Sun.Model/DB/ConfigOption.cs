using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
    //网站配置选项表
    [TableName("Web_ConfOption")]
    [PrimaryKey("OptionId",true)]
    [ExplicitColumns]
    public class ConfigOption
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int OptionId
        {
            get;
            set;
        }
        /// <summary>
        /// 关联配置Id
        /// </summary>
        [Column]
        public int ConfigId
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Column]
        public string OptionName
        {
            get;
            set;
        }
        /// <summary>
        /// 值
        /// </summary>
        [Column]
        public string Values
        {
            get;
            set;
        }
    }
}
