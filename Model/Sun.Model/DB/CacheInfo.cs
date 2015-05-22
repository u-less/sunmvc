using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DB
{
    [TableName("CacheInfo")]
    [PrimaryKey("CacheId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class CacheInfo
    {
        /// <summary>
        /// 缓存编号
        /// </summary>
        [Column]
        public int CacheId
        {
            get;
            set;
        } 
        /// <summary>
        /// 实体名称
        /// </summary>
        [Column]
        public string CacheType
        {
            get;
            set;
        }
        /// <summary>
        /// 键
        /// </summary>
        [Column]
        public string KeyId
        {
            get;
            set;
        }
        /// <summary>
        /// 键值
        /// </summary>
        [Column]
        public string KeyValue
        {
            get;
            set;
        }
        /// <summary>
        /// 更新时间
        /// </summary>
        [Column]
        public DateTime UpdateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 预更新时间
        /// </summary>
        [Column]
        public DateTime BookUpdateTime
        {
            get;
            set;
        }
    }
}
