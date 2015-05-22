using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fang.BLL.DBOperate;

namespace Fang.BLL.Config
{
    public partial class CustomConfig
    {
        /// <summary>
        /// 允许上传的图片大小(M)
        /// </summary>
        public static float CanUploadImgSize
        {
            get
            {
                return float.Parse(WebConfigOp.GetConfigValueByKey("CanUploadImgSize"));
            }
        }
        /// <summary>
        /// 允许上传的文件大小(M)
        /// </summary>
        public static float CanUploadFileSize
        {
            get
            {
                return float.Parse(WebConfigOp.GetConfigValueByKey("CanUploadFileSize"));
            }
        }
        /// <summary>
        /// 文件上传类型(0:本地,1:mongoDb上传)
        /// </summary>
        public static int UploadType
        {
            get
            {
                return int.Parse(WebConfigOp.GetRadioConfigValueByKey("FileUploadType"));
            }
        }
        /// <summary>
        /// 文件上传地址
        /// </summary>
        public static string FileUpUrl
        {
            get
            {
                return WebConfigOp.GetConfigValueByKey("FileUpUrl");
            }
        }
        /// <summary>
        /// 图片上传默认压缩质量
        /// </summary>
        public static int ImgQuality
        {
            get
            {
                return int.Parse(WebConfigOp.GetConfigValueByKey("ImgQuality"));
            }
        }
        /// <summary>
        /// 文本编辑器本地图片存放位置
        /// </summary>
        public static string EditorImgPath
        {
            get
            {
                return "/Upload/EditorImg/";
            }
        }

        /// <summary>
        /// 文本编辑器本地附件存放位置
        /// </summary>
        public static string EditorFilePath
        {
            get
            {
                return "/Upload/EditorFile/";
            }
        }
        /// <summary>
        /// 允许上传的图片格式
        /// </summary>
        public static IEnumerable<string> ImgExts
        {
            get
            {
                return WebConfigOp.GetConfigValueByKey("ImgExt").Split(',').Select(o => o.Trim());
            }
        }
        /// <summary>
        /// 允许上传文件格式
        /// </summary>
        public static string[] FileExts
        {
            get
            {
                return WebConfigOp.GetConfigValueByKey("DocZipExt").Split(',').Select(o => o.Trim()).ToArray();
            }
        }
        /// <summary>
        /// 允许上传的视频格式
        /// </summary>
        public static string[] VideoExts
        {
            get
            {
                return WebConfigOp.GetConfigValueByKey("VideoExt").Split(',').Select(o => o.Trim()).ToArray();
            }
        }
        /// <summary>
        /// 文件数据库链接地址
        /// </summary>
        public static List<Fang.BLL.MongoDb.SunMongoServer> FilesServer
        {
            get
            {
                return WebConfigOp.GetConfigObjectByKey<List<Fang.BLL.MongoDb.SunMongoServer>>("FilesServer");
            }
        }
        /// <summary>
        /// 文件服务器域名地址
        /// </summary>
        public static string FilesDomain
        {
            get
            {
                return WebConfigOp.GetConfigValueByKey("FilesDomain");
            }
        }
        /// <summary>
        /// 图片和文件上传存储方式
        /// </summary>
        public static string FileUploadType
        {
            get
            {
                return WebConfigOp.GetRadioConfigValueByKey("FileUploadType");
            }
        }
        /// <summary>
        /// 水印图片编号
        /// </summary>
        public static string WaterImg
        {
            get
            {
                return WebConfigOp.GetConfigValueByKey("WaterImgId");
            }
        }
        /// <summary>
        /// 错误图片编号
        /// </summary>
        public static string ImgError
        {
            get
            {
                return WebConfigOp.GetConfigValueByKey("ImgError");
            }
        }
    }
}
