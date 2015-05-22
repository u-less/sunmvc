using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Sun.Framework.SunImg
{
    public class CatchImg
    {
        public string SourceUrl { get; set; }
        public string ServerUrl { get; set; }
        public string State { get; set; }

        private HttpServerUtilityBase Server { get; set; }


        public CatchImg(string sourceUrl, HttpServerUtilityBase server)
        {
            this.SourceUrl = sourceUrl;
            this.Server = server;
        }
        /// <summary>
        /// 抓取图片
        /// </summary>
        /// <param name="dicPath">图片保存目录</param>
        /// <param name="userType">用户类型</param>
        /// <param name="userId">用户编号</param>
        /// <returns></returns>
        public CatchImg Fetch(string dicPath, string userType = null, string userId = null)
        {
            var request = HttpWebRequest.Create(this.SourceUrl) as HttpWebRequest;
            using (var response = request.GetResponse() as HttpWebResponse)
            {
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    State = "Url returns " + response.StatusCode + ", " + response.StatusDescription;
                    return this;
                }
                if (response.ContentType.IndexOf("image") == -1)
                {
                    State = "Url is not an image";
                    return this;
                }
                ServerUrl = dicPath + DateTime.Now.ToString("yyyy/MM/");
                if (!string.IsNullOrEmpty(userType) && !string.IsNullOrEmpty(userId))
                    ServerUrl += "[" + userType + "-" + userId + "]";
                var savePath = Server.MapPath(ServerUrl);
                if (!Directory.Exists(Path.GetDirectoryName(savePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                }
                try
                {
                    var stream = response.GetResponseStream();
                    var reader = new BinaryReader(stream);
                    byte[] bytes;
                    using (var ms = new MemoryStream())
                    {
                        byte[] buffer = new byte[4096];
                        int count;
                        while ((count = reader.Read(buffer, 0, buffer.Length)) != 0)
                        {
                            ms.Write(buffer, 0, count);
                        }
                        bytes = ms.ToArray();
                    }
                    File.WriteAllBytes(savePath, bytes);
                    State = "SUCCESS";
                }
                catch (Exception e)
                {
                    State = "抓取错误：" + e.Message;
                }
                return this;
            }
        }
    }
}
