using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Message;

namespace Sun.Framework.Message
{
    public class SunSMS:ISMS
    {
        SMSUser User;
        public SunSMS(SMSUser user)
        {
            this.User = user;
        }
        public int sendOnce(string mobile, string content)
        {
           return SMSHelper.sendOnce(User, mobile, content);
        }

        public int sendBatch(string mobile, string content)
        {
            return SMSHelper.sendBatch(User, mobile, content);
        }
    }
}
