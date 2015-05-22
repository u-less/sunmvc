using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.DBContext;
using Sun.Model.DB;
using Sun.Model.Common;
using Sun.Model.DBExtensions;

namespace Sun.BaseOperate.Interface
{
    public interface IRoleLimitOp : IDBOperate<SysRoleLimit>
    {
        /// <summary>
        /// 给指定角色分配权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="setRoleId">分配者角色ID</param>
        string GetLimitAboutRoleJson(int roleId, int setRoleId);
        /// <summary>
        /// 获取当前用户角色在指定模块拥有的权限
        /// </summary>
        /// <param name="ModuleID">模块ID</param>
        /// <returns></returns>
        List<int> GetLimitsByModuleAndRole(string moduleKey, string roleId);
        /// <summary>
        /// 获取当前角色的所有权限信息
        /// </summary>
        /// <param name="RoleID">角色ID</param>
        /// <returns></returns>
        IEnumerable<Limit> GetRoleLimits(int roleId);
        /// <summary>
        /// 获取所有权限(包含指定角色是否拥有某一权限)
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="setRoleId">分配者角色Id</param>
        /// <returns></returns>
        IEnumerable<LimitAoutRole> GetLimitsAboutRole(int roleId, int setRoleId = 0);
        /// <summary>
        /// 获取角色的所有模块的权限信息(包含指定角色是否拥有某一模块的权限)
        /// </summary>
        /// <param name="role"></param>
        /// <param name="setRoleId">分配者角色Id</param>
        /// <returns></returns>
        IEnumerable<ModuleAoutRole> GetModuleAboutRole(int roleId, int setRoleId = 0);
        /// <summary>
        /// 判断该角色是否拥有该权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// /// <param name="limitId">权限Id</param>
        /// <returns></returns>
        bool RoleLimitExists(int roleId, int limitId);
        /// <summary>
        /// 判断该角色是否拥有指定模块的权限
        /// </summary>
        /// <param name="roleId">角色id</param>
        /// <param name="moduleId">模块id</param>
        /// <returns></returns>
        bool RoleModuleExists(int roleId, int moduleId);
        /// <summary>
        /// 删除或新增角色的权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="limitIds">权限Id集合</param>
        /// <param name="AdminId">操作着</param>
        /// <param name="AddTime">操作时间</param>
        void SetRoleLimit(int roleId, string[] limitIds);
        /// <summary>
        /// 新增或删除角色的模块权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="modules">模块Id集合</param>
        /// <param name="AdminId">操作人</param>
        /// <param name="AddTime">操作时间</param>
        void SetRoleModule(int roleId, string[] modules);
        /// <summary>
        /// 删除指定角色的指定权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="limitId">权限Id</param>
        void DeleteRoleLimit(int roleId, int limitId);
        /// <summary>
        /// 删除指定角色的模块权限
        /// </summary>
        /// <param name="roleId">角色Id</param>
        /// <param name="moduleId">模块Id</param>
        void DeleteRoleModule(int roleId, int moduleId);
    }
}
