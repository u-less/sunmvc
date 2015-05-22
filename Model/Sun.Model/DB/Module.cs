using System;
using Sun.Core.DBContext;
using ProtoBuf;

namespace Sun.Model.DB
{

    //模块信息表
    [TableName("sys_module")]
    [PrimaryKey("ModuleId", true)]
    [ExplicitColumns]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class Module
    {

        /// <summary>
        /// 编号
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
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 上级模块
        /// </summary>
        [Column]
        public int ParentId
        {
            get;
            set;
        }
        /// <summary>
        /// 模块手动标识
        /// </summary>
        [Column]
        public string ModuleKey
        {
            get;
            set;
        }
        /// <summary>
        /// 模块类别
        /// </summary>
        [Column]
        public int TypeId
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
        /// 模块图标
        /// </summary>
        [Column]
        public string Icon
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
        /// 关联值
        /// </summary>
        [Column]
        public string ModuleValue
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
        /// <summary>
        /// 标题
        /// </summary>
        [Column]
        public string Title
        {
            get;
            set;
        }
        /// <summary>
        /// 关键字
        /// </summary>
        [Column]
        public string KeyWords
        {
            get;
            set;
        }
        /// <summary>
        /// 摘要
        /// </summary>
        [Column]
        public string Description
        {
            get;
            set;
        }
    }
}
