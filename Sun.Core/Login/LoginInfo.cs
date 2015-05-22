using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Sun.Core.Login
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class LoginInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public int UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 用户类别
        /// </summary>
        public int Type
        {
            get;
            set;
        }
        /// <summary>
        /// 用户等级
        /// </summary>
        public int Level
        {
            get;
            set;
        }
        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName
        {
            get;
            set;
        }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string HeadImg
        {
            get;
            set;
        }
        /// <summary>
        /// 角色编号
        /// </summary>
        public int RoleId
        {
            get;
            set;
        }
        /// <summary>
        /// 机构编号
        /// </summary>
        public string OrganId
        {
            get;
            set;
        }
    }
}
