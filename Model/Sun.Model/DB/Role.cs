using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
   //角色信息表
    [TableName("Sys_Role")]
    [PrimaryKey("RoleId", true)]
    [ExplicitColumns]
    public class Role
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int RoleId
        {
            get;
            set;
        }
        /// <summary>
        /// 机构编号
        /// </summary>
        [Column]
        public int OrganId
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
        /// 添加人
        /// </summary>
        [Column]
        public int AdminId
        {
            get;
            set;
        }
        /// <summary>
        /// 添加时间
        /// </summary>
        [Column]
        public DateTime AddTime
        {
            get;
            set;
        }
        /// <summary>
        /// 显示顺序
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