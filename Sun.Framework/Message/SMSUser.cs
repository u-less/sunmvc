using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Framework.Message
{
    /// <summary>
    /// 短信服务配置
    /// </summary>
    public class SMSUser
    {
        /// <summary>
        /// 发送地址
        /// </summary>
        public string Url
        {
            get;
            set;
        }
        /// <summary>
        /// 账号
        /// </summary>
        public string Account
        {
            get;
            set;
        }
        /// <summary>
        /// 密钥
        /// </summary>
        public string AuthKey
        {
            get;
            set;
        }
        /// <summary>
        /// 通道
        /// </summary>
        public uint CGId
        {
            get;
            set;
        }
    }
}
