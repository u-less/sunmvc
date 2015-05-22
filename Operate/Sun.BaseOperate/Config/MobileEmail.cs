using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.BaseOperate.DB;
using Sun.Framework.Message;

namespace Sun.BaseOperate.Config
{
    public partial class CustomConfig
    {
        /// <summary>
        /// 邮箱服务信息
        /// </summary>
        public static EmailUser EmailConfig
        {
            get
            {
                return ConfigOp.GetConfigObjectByKey<EmailUser>("EmailConfig");
            }
        }
        /// <summary>
        /// 短信账号
        /// </summary>
        public static SMSUser SMSConfig
        {
            get
            {
                return ConfigOp.GetConfigObjectByKey<SMSUser>("SMSConfig");
            }
        }
    }
}
