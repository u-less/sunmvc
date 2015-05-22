using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Sun.Framework.Security
{
   public class AESEncrypt
    {
        //向量
        private static byte[] iv = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF, 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
        /// <summary>
        /// DES加密字符串
        /// </summary>
        /// <param name="encryptString">待加密的字符串</param>
        /// <param name="secretKey">8位长度密钥</param>
        /// <returns>加密成功返回加密后的字符串,失败返回源串</returns>
        public static string Encode(string encryptString, string secretKey)
        {
            Rijndael aes = Rijndael.Create();
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(secretKey.Substring(0, 8));
                byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(rgbKey, iv), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Convert.ToBase64String(mStream.ToArray()).Replace('+', '_');
            }
            catch
            {
                return encryptString;
            }
            finally
            {
                aes.Clear();
            }
        }

        /// <summary>
        /// DES解密字符串
        /// </summary>
        /// <param name="decryptString">待解密的字符串</param>
        /// <param name="secretKey">8位长度密钥</param>
        /// <returns>解密成功返回解密后的字符串,失败返源串</returns>
        public static string Decode(string decryptString, string secretKey)
        {
            Rijndael aes = Rijndael.Create();
            try
            {
                byte[] rgbKey = Encoding.UTF8.GetBytes(secretKey.Substring(0, 8));
                byte[] inputByteArray = Convert.FromBase64String(decryptString.Replace('_', '+'));
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, aes.CreateDecryptor(rgbKey, iv), CryptoStreamMode.Write);
                cStream.Write(inputByteArray, 0, inputByteArray.Length);
                cStream.FlushFinalBlock();
                return Encoding.UTF8.GetString(mStream.ToArray());
            }
            catch
            {
                return string.Empty;
            }
            finally
            {
                aes.Clear();
            }
        }
    }
}
