using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
   //地区表
    [TableName("Sys_Region")]
    [PrimaryKey("RegionId", false)]
    [ExplicitColumns]
    public class Region
    {
        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int RegionId
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
        /// 上级Id
        /// </summary>
        [Column]
        public int ParentId
        {
            get;
            set;
        }
        /// <summary>
        /// 邮编
        /// </summary>
        [Column]
        public string PostCode
        {
            get;
            set;
        }
        /// <summary>
        /// 区号
        /// </summary>
        [Column]
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 拼音
        /// </summary>
        [Column]
        public string Spell
        {
            get;
            set;
        }
        /// <summary>
        /// 顺序
        /// </summary>
        [Column]
        public int SortIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 热点城市
        /// </summary>
        [Column]
        public int IsHot
        {
            get;
            set;
        }
    }
}