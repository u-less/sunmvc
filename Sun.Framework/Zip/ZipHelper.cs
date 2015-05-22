using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Ionic.Zip;

namespace Sun.Framework.Zip
{
    public class ZipHelper
    {
        /// <summary>
        /// 压缩zip
        /// </summary>
        /// <param name="fileToZips">文件路径集合</param>
        /// <param name="zipedFile">想要压成zip的文件名</param>
        public static void Zip(string[] fileToZips, string zipedFile)
        {
            using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(zipedFile, Encoding.Default))
            {
                foreach (string fileToZip in fileToZips)
                {
                    using (FileStream fs = new FileStream(fileToZip, FileMode.Open, FileAccess.ReadWrite))
                    {
                        byte[] buffer = new byte[fs.Length];
                        fs.Read(buffer, 0, buffer.Length);
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        zip.AddEntry(fileName, buffer);
                    }
                }
                zip.Save();
            }
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="savePath"></param>
        public static void Extract(string zipPath, string savePath)
        {
            using (ZipFile zip = ZipFile.Read(zipPath))
            {
                foreach (ZipEntry e in zip)
                {
                    e.Extract(savePath);
                }
            }
        }
    }
}
