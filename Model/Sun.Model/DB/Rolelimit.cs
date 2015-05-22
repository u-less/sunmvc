using System;
using Sun.Core.DBContext;

namespace Sun.Model.DB
{
   //角色权限表
    [TableName("Sys_RoleLimit")]
    [PrimaryKey("RLId", true)]
    [ExplicitColumns]
    public class SysRoleLimit
    {

        /// <summary>
        /// 编号
        /// </summary>
        [Column]
        public int RLId
        {
            get;
            set;
        }
        /// <summary>
        /// 角色
        /// </summary>
        [Column]
        public int RoleId
        {
            get;
            set;
        }
        /// <summary>
        /// 模块
        /// </summary>
        [Column]
        public int ModuleId
        {
            get;
            set;
        }
        /// <summary>
        /// 权限
        /// </summary>
        [Column]
        public int LimitId
        {
            get;
            set;
        }
    }
}