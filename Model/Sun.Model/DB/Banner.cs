using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DB
{
    //Banner信息表
    [TableName("Web_Banner")]
    [PrimaryKey("BannerId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class Banner
    {
        /// <summary>
        /// Banner编号
        /// </summary>
        [Column]
        public int BannerId
        {
            get;
            set;
        }
        /// <summary>
        /// Banner标题
        /// </summary>
        [Column]
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// Banner链接
        /// </summary>
        [Column]
        public string LinkUrl
        {
            get;
            set;
        }

        /// <summary>
        /// Banner类型
        /// </summary>
        [Column]
        public int Types
        {
            get;
            set;
        }
        /// <summary>
        /// Banner位置
        /// </summary>
        [Column]
        public int Layout
        {
            get;
            set;
        }
        /// <summary>
        ///顺序
        /// </summary>
        [Column]
        public int SortIndex
        {
            get;
            set;
        }
        /// <summary>
        ///状态
        /// </summary>
        [Column]
        public bool IsUsable
        {
            get;
            set;
        }
        /// <summary>
        ///关联Id
        /// </summary>
        [Column]
        public int RelateId
        {
            get;
            set;
        }
        /// <summary>
        /// 关联值
        /// </summary>
        [Column]
        public string Value
        {
            get;
            set;
        }
        /// <summary>
        ///图片地址
        /// </summary>
        [Column]
        public string PicSrc
        {
            get;
            set;
        }
    }
}
