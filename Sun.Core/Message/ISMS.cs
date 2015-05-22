using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Message
{
    public interface ISMS
    {
        /// <summary>
        /// 单条信息群发
        /// </summary>
        /// <param name="mobile">手机号码, 如有多个使用逗号分隔, 支持1~3000个号码</param>
        /// <param name="content">内容, 500字以内</param>
        /// <returns></returns>
       int sendOnce(string mobile, string content);
        /// <summary>
        /// 多条信息群发
        /// </summary>
       /// <param name="mobile">手机号码, 如有多个使用逗号分隔, 支持2~100个号码</param>
       /// <param name="content">内容, 500字以内, 多个用{|}分隔</param>
        /// <returns></returns>

       int sendBatch(string mobile, string content);
    }
}
