using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;

namespace Sun.Framework.SunImg
{
    public class SunImgFormat
    {
        public static string[] bmpExts = new string[]{"BMP","DIB","RLE"};
        public static string[] jpegExts = new string[] {"JPG","JPEG","JPE","JFIF"};
        public static string[] tiffExts = new string[] { "TIF","TIFF"};
        public static string[] pngExts = new string[] { "PNG" };
        /// <summary>
        /// 获取image编码解码器类型
        /// </summary>
        /// <param name="ext">图片后缀</param>
        /// <returns></returns>
        public static ImageCodecInfo GetImageCodecInfo(string ext)
        {
            ext = ext.ToUpper().Trim('.');
            ImageCodecInfo[] arrayICI = ImageCodecInfo.GetImageEncoders();
            ImageCodecInfo codecInfo = null;
            if (jpegExts.Contains(ext))
                codecInfo = arrayICI.SingleOrDefault(c => c.FormatDescription.Equals("JPEG"));
            else if (ext == "GIF")
                codecInfo = arrayICI.SingleOrDefault(c => c.FormatDescription.Equals("GIF"));
            else if (ext == "PNG")
                codecInfo = arrayICI.SingleOrDefault(c => c.FormatDescription.Equals("PNG"));
            else if (bmpExts.Contains(ext))
                codecInfo = arrayICI.SingleOrDefault(c => c.FormatDescription.Equals("BMP"));
            else if (tiffExts.Contains(ext))
                codecInfo = arrayICI.SingleOrDefault(c => c.FormatDescription.Equals("TIFF"));
            return codecInfo;
        }
        /// <summary>
        /// 获取图片文本类型
        /// </summary>
        /// <param name="ext">图片后缀</param>
        /// <returns></returns>
        public static string GetImageContentType(string ext)
        {
            ext = ext.ToUpper();
            string contentType = null;
            if (jpegExts.Contains(ext))
                contentType = "image/jpeg";
            else if (ext == "GIF")
                contentType = "image/gif";
            else if (ext == "PNG")
                contentType = "image/png";
            else if (bmpExts.Contains(ext))
                contentType = "image/bmp";
            else if (tiffExts.Contains(ext))
                contentType = "image/tiff";
            return contentType;
        }
    }
}
