using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DB
{
   //图库信息表
    [TableName("PicStore")]
    [PrimaryKey("PicId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class PicStore
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int PicId
        {
            get;
            set;
        }
        /// <summary>
        /// 类别
        /// </summary>
        [Column]
        public int PicType
        {
            get;
            set;
        }
        /// <summary>
        /// 关联编号
        /// </summary>
        [Column]
        public int AboutId
        {
            get;
            set;
        }
        /// <summary>
        /// 图片地址
        /// </summary>
        [Column]
        public string ImgSrc
        {
            get;
            set;
        }
        /// <summary>
        /// 标题
        /// </summary>
        [Column]
        public string Description
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
        /// 状态
        /// </summary>
        [Column]
        public int State
        {
            get;
            set;
        }
    }
}