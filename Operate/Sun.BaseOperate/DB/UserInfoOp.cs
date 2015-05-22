using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core;
using Sun.Core.Caching;
using Sun.BaseOperate.Interface;
using Sun.Model.DB;
using Sun.Model.DBExtensions;
using Sun.Model.Common;
using Sun.BaseOperate.DbContext;
using Sun.Core.Ioc;
using Sun.Framework.Security;
using Sun.BaseOperate.Config;
using Sun.Core.DBContext;

namespace Sun.BaseOperate.DB
{
    [IocExport(typeof(IUserInfoOp), true)]
   public class UserInfoOp:IUserInfoOp
    {
        public UserInfoOp(IModelCacheFac<UserInfo> CacheOp)
        {
            this.CacheOp = CacheOp;
        }
        public Core.DBContext.Page<UserGrid> GetPageList(int page = 1, int rows = 30, string username = null, string sex = null, string states = null, string usertype = null, string regionid = null, string organid = null, string roleid = null, DateTime? starttime = null, DateTime? endtime = null)
        {
            var sqlstr = "SELECT a.*,b.\"name\" regionname,c.\"name\" rolename,array_to_string(ARRAY(select d.organname from sys_organ d WHERE regexp_split_to_array(a.organids,',')@> array[d.organid]::text[]),',') organname FROM sys_userinfo a LEFT JOIN sys_region b ON a.regionid=b.regionid LEFT JOIN sys_role c ON a.roleid=c.roleid";

            Sql sql = new Sql(sqlstr);
            if (!String.IsNullOrEmpty(username))
            {
                sql.Where("(a.username Like @0 or a.loginid Like @0 or a.nickname Like @0)", "%" + username.Trim() + "%");
            }
            if (!String.IsNullOrEmpty(sex))
            {
                sql.Where("a.sex=@0", sex);
            }
            if (!String.IsNullOrEmpty(states))
            {
                sql.Where("a.states=@0", states);
            }
            if (!String.IsNullOrEmpty(usertype))
            {
                sql.Where("a.usertype=@0", usertype);
            }
            if (!String.IsNullOrEmpty(regionid))
            {
                sql.Where("a.regionid=@0", regionid);
            }
            if (!String.IsNullOrEmpty(organid))
            {
                sql.Where("regexp_split_to_array(a.organids,',') @> array[" + organid + "]::text[]");
            }
            if (!String.IsNullOrEmpty(roleid))
            {
                sql.Where("a.roleid=@0", roleid);
            }
            if (starttime != null || endtime != null)
            {
                if (starttime == null)
                    starttime = new DateTime(1970, 1, 1);
                if (endtime == null)
                    endtime = DateTime.Now;
                sql.Where("a.lastlogintime between '" + starttime + "' and '" + endtime + "'");
            }

            sql.OrderBy("a.loginid");
            return Context.Instance.Page<UserGrid>(page, rows, sql);
        }

        public bool AccountExist(string loginId)
        {
            return Context.Instance.ExecuteScalar<int>("select count(*) from sys_userinfo where loginid=@0 or (IsBindEmail='true' AND Email=@0) or (IsBindPhone='true' AND PhoneNumber=@0)", loginId.ToLower()) > 0;
        }

        public bool PhoneNumberExist(string number)
        {
            return Context.Instance.ExecuteScalar<int>("select count(*) from sys_userinfo where IsBindPhone='true' AND PhoneNumber=@0", number) > 0;
        }

        public bool EmailExist(string email)
        {
            return Context.Instance.ExecuteScalar<int>("select count(*) from sys_userinfo where IsBindEmail='true' AND Email=@0", email.ToLower()) > 0;
        }

        public bool BindEmail(int userId, string email)
        {
            return Context.Instance.Update<UserInfo>("set isbindemail=@0,email=@2 where userid=@1 and (SELECT count(*) from sys_userinfo where email='@2' AND isbindemail is true)=0", true, userId, email) > 0;
        }

        public bool BindPhone(int userId, string phone)
        {
            return Context.Instance.Update<UserInfo>("set isbindphone=@0,phonenumber=@2 where userid=@1 and (SELECT count(*) from sys_userinfo where phonenumber='@2' AND isbindphone is true)=0", true, userId, phone) > 0;
        }

        public string FindPwdDataEncode(int userid, string account)
        {
            var endTime = DateTime.Now.AddHours(1);
            var str = account + "|" + endTime.ToString();
            var key = SecretKey.GetKey(CustomConfig.FindPwdEmailTicks, userid.ToString());
            var result = AESEncrypt.Encode(str, key);
            result = result + "$" + userid.ToString();
            return result;
        }

        public bool FindPwdDataDecode(string data, out string account, out int userid)
        {
            try
            {
                var adminStrs = data.Split('$');
                if (adminStrs.Length == 2)
                {
                    userid = int.Parse(adminStrs[1]);
                    var key = SecretKey.GetKey(CustomConfig.FindPwdEmailTicks, userid.ToString());
                    SecretKey.KeyDelete(CustomConfig.FindPwdEmailTicks, userid.ToString());
                    var loginStrs = AESEncrypt.Decode(adminStrs[0], key).Split('|');
                    if (loginStrs.Length == 2)
                    {
                        var expireTime = DateTime.Parse(loginStrs[1]);
                        if (expireTime > DateTime.Now)
                        {
                            account = loginStrs[0];
                            return true;
                        }
                    }
                }
            }
            catch { }
            account = null;
            userid = 0;
            return false;
        }

        public bool SetLastLoginInfo(int userId, DateTime loginTime, string iPAddress)
        {
            return Context.Instance.Update<UserInfo>("set LastLoginTime=@0,set LastLoginIp=@1 where userid=@3", loginTime, iPAddress, userId) > 0;
        }

        public IEnumerable<KeyValue> GetUserTypes()
        {
            var edata = EnumExtend<UserType>.GetEnumKeyName();
            return edata.Select(o => new KeyValue { key = o.Key.ToString(), value = o.Value });
        }

        public IEnumerable<KeyValue> GetUserStatus()
        {
            var edata = EnumExtend<UserStatus>.GetEnumKeyName();
            return edata.Select(o => new KeyValue { key = o.Key.ToString(), value = o.Value });
        }

        public UserHeadInfo GetUserHeadInfoById(int id)
        {
            return Context.Instance.SingleOrDefault<UserHeadInfo>("SELECT a.userid,a.nickname,a.Sex,a.HeadImg,a.Fans,a.Focus,a.level,a.regionid,b.sign from sys_userinfo a INNER JOIN sys_userdetail b on a.userid=b.userid  where a.userid=@0", id);
        }

        public int UpdatePassword(string newPassword, int userId)
        {
           return Context.Instance.Update<UserInfo>("set PassWord=@0 where userid=@1", Md5Encrypt.PasswordEncode(newPassword), userId);
        }

        public IModelCacheFac<UserInfo> CacheOp
        {
            get;
            set;
        }

        public object Add(UserInfo entity)
        {
            var context = Context.Instance;
            context.BeginTransaction();
            var id = 0;
            try
            {
                id = Convert.ToInt32(context.Insert(entity));
                var detail = new UserDetail();
                detail.UserId = id;
                detail.Birthday = DateTime.Now;
                var userPoint = new UserPoint();
                userPoint.UserId = id;
                context.Insert(detail);
                context.Insert(userPoint);
                context.CompleteTransaction();
            }
            catch
            {
                context.AbortTransaction();
            }

            return id;
        }

        public int Delete(object id)
        {
            return Context.Instance.Update<UserInfo>("set states=@0 where userid=@1",UserStatus.Lock, id);
        }

        public int Update(UserInfo entity)
        {
            var r = Context.Instance.Update(entity);
            if (r > 0) CacheOp.UpdateAll();
            return r;
        }

        public int Update(UserInfo entity, string[] columns)
        {
            var r = Context.Instance.Update(entity, columns);
            if (r > 0) CacheOp.UpdateAll();
            return r;
        }

        public UserInfo GetModelById(object id)
        {
            return Context.Instance.SingleOrDefault<UserInfo>(id);
        }
    }
}
