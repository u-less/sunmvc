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
    public interface IUserInfoOp:IDBOperate<UserInfo>
    {
        //获取分页数据
        Page<UserGrid> GetPageList(int page = 1, int rows = 30, string username = null, string sex = null, string states = null, string usertype = null, string regionid = null, string organid = null, string roleid = null, DateTime? starttime = null, DateTime? endtime = null);
        /// <summary>
        /// 判断账号是否存在
        /// </summary>
        /// <param name="loginId">账号</param>
        /// <returns></returns>
        bool AccountExist(string loginId);
        /// <summary>
        /// 判断电话号码是否被绑定
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        bool PhoneNumberExist(string number);
        /// <summary>
        /// 判断邮箱是否被绑定
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        bool EmailExist(string email);
        /// <summary>
        /// 绑定邮箱
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        bool BindEmail(int userId, string email);
        /// <summary>
        /// 绑定手机
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        bool BindPhone(int userId, string phone);
        /// <summary>
        /// 加密数据发送给用户进行密码重置
        /// </summary>
        /// <param name="loginId">账号</param>
        /// <param name="adminId">账号编号</param>
        /// <returns>加密后的数据</returns>
        string FindPwdDataEncode(int userid, string account);
        /// <summary>
        /// 解密用户进行密码重置的数据
        /// </summary>
        /// <param name="data">用户提交的数据</param>
        /// <param name="account">解密后的账号</param>
        /// <param name="userid">解密后的账号编号</param>
        /// <returns>数据是否合法</returns>
        bool FindPwdDataDecode(string data, out string account, out int userid);
        /// <summary>
        /// 记录用户最后登录信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginTime"></param>
        /// <param name="iPAddress"></param>
        /// <returns></returns>
        bool SetLastLoginInfo(int userId, DateTime loginTime, string iPAddress);
        /// <summary>
        /// 获取用户类别数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> GetUserTypes();
        /// <summary>
        /// 获取用户状态数据
        /// </summary>
        /// <returns></returns>
        IEnumerable<KeyValue> GetUserStatus();
        /// <summary>
        /// 获取会员个人中心头部需要的会员信息
        /// </summary>
        /// <param name="id">会员编号</param>
        /// <returns></returns>
        UserHeadInfo GetUserHeadInfoById(int id);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="newPassword"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        int UpdatePassword(string newPassword, int userId);
    }
}
