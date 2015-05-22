using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Sun.Framework.Security
{
    public class Md5Encrypt
    {
        /// <summary>
        /// 加密
        /// </summary>
        /// <param name="str">需要加密字符串</param>
        /// <returns></returns>
        public static string Encode(string str)
        {
            //将输入转换为ASCII 字符编码
            ASCIIEncoding enc = new ASCIIEncoding();
            //将字符串转换为字节数组
            byte[] buffer = enc.GetBytes(str);
            //创建MD5实例
            MD5 md5 = new MD5CryptoServiceProvider();
            //进行MD5加密
            byte[] hash = md5.ComputeHash(buffer);
            StringBuilder sb = new StringBuilder();
            //拼装加密后的字符
            for (int i = 0; i < hash.Length; i++)
            {
                sb.AppendFormat("{0:x2}", hash[i]);
            }
            //输出加密后的字符串
            return sb.ToString();
        }
        /// <summary>
        /// 密码加密
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string PasswordEncode(string Password)
        {
            return Encode(Encode("sun" + Password) + "sun");
        }
    }
}
