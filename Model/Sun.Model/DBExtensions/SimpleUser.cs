using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace Sun.Model.DBExtensions
{
    /// <summary>
    /// 用户昵称头像信息
    /// </summary>
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class SimpleUser
    {
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
        /// 用户等级
        /// </summary>
        public string Level
        {
            get;
            set;
        }
    }
}
