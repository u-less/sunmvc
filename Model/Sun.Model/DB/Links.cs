using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
    //友情链接
    [TableName("Web_Links")]
    [PrimaryKey("LinkId", true)]
    [ExplicitColumns]
    public class Links
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int LinkId
        {
            get;
            set;
        }
        /// <summary>
        /// 关联模块
        /// </summary>
        [Column]
        public int ModuleId
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Column]
        public string LinkName
        {
            get;
            set;
        }
        /// <summary>
        /// 链接地址
        /// </summary>
        [Column]
        public string LinkUrl
        {
            get;
            set;
        }
        /// <summary>
        /// 链接logo
        /// </summary>
        [Column]
        public string LinkPic
        {
            get;
            set;
        }
        /// <summary>
        /// 顺序
        /// </summary>
        [Column]
        public int OrderIndex
        {
            get;
            set;
        }
        /// <summary>
        /// 可用
        /// </summary>
        [Column]
        public bool IsUsable
        {
            get;
            set;
        }
    }
}