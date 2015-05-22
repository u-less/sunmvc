using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DB
{
  //系统字典表
    [TableName("Sys_Dict")]
    [PrimaryKey("DictId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class Dict
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int DictId
        {
            get;
            set;
        }
        /// <summary>
        /// 名称
        /// </summary>
        [Column]
        public string DictName
        {
            get;
            set;
        }
        /// <summary>
        /// 编码
        /// </summary>
        [Column]
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 值
        /// </summary>
        [Column]
        public string DataValue
        {
            get;
            set;
        }
        /// <summary>
        /// 类别
        /// </summary>
        [Column]
        public int TypeId
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
        /// 是否可用
        /// </summary>
        [Column]
        public bool IsUsable
        {
            get;
            set;
        }
    }
}