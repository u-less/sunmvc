using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.BaseOperate.Interface;
using Sun.Core.Ioc;
using Autofac;

namespace Sun.BaseOperate.Config
{
    public partial class CustomConfig
    {
        public static IConfigOp ConfigOp = WebIoc.Container.Resolve<IConfigOp>();
        /// <summary>
        /// 允许上传的图片大小(M)
        /// </summary>
        public static float CanUploadImgSize
        {
            get
            {
                return float.Parse(ConfigOp.GetConfigValueByKey("CanUploadImgSize"));
            }
        }
        /// <summary>
        /// 允许上传的文件大小(M)
        /// </summary>
        public static float CanUploadFileSize
        {
            get
            {
                return float.Parse(ConfigOp.GetConfigValueByKey("CanUploadFileSize"));
            }
        }
        /// <summary>
        /// 文件上传类型(0:本地,1:mongoDb上传)
        /// </summary>
        public static int UploadType
        {
            get
            {
                return int.Parse(ConfigOp.GetRadioConfigValueByKey("FileUploadType"));
            }
        }
        /// <summary>
        /// 文件上传地址
        /// </summary>
        public static string FileUpUrl
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("FileUpUrl");
            }
        }
        /// <summary>
        /// 图片上传默认压缩质量
        /// </summary>
        public static int ImgQuality
        {
            get
            {
                return int.Parse(ConfigOp.GetConfigValueByKey("ImgQuality"));
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
                return ConfigOp.GetConfigValueByKey("ImgExt").Split(',').Select(o => o.Trim());
            }
        }
        /// <summary>
        /// 允许上传文件格式
        /// </summary>
        public static string[] FileExts
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("DocZipExt").Split(',').Select(o => o.Trim()).ToArray();
            }
        }
        /// <summary>
        /// 允许上传的视频格式
        /// </summary>
        public static string[] VideoExts
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("VideoExt").Split(',').Select(o => o.Trim()).ToArray();
            }
        }
        /// <summary>
        /// 文件服务器域名地址
        /// </summary>
        public static string FilesDomain
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("FilesDomain");
            }
        }
        /// <summary>
        /// 图片和文件上传存储方式
        /// </summary>
        public static string FileUploadType
        {
            get
            {
                return ConfigOp.GetRadioConfigValueByKey("FileUploadType");
            }
        }
        /// <summary>
        /// 水印图片编号
        /// </summary>
        public static string WaterImg
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("WaterImgId");
            }
        }
        /// <summary>
        /// 错误图片编号
        /// </summary>
        public static string ImgError
        {
            get
            {
                return ConfigOp.GetConfigValueByKey("ImgError");
            }
        }
    }
}
