using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Sun.Framework.SunImg
{
    public class SunImage
    {
        #region 正方型裁剪并缩放

        /// <summary>
        /// 正方型裁剪
        /// 以图片中心为轴心，截取正方型，然后等比缩放
        /// 用于头像处理
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="side">指定的边长（正方型）</param>
        /// <param name="quality">质量（范围0-100）</param>
        ///<param name="imgExt">图片后缀</param>
        public static MemoryStream CutForSquare(System.IO.Stream fromFile, int side, int quality, string imgExt)
        {

            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);
            fromFile.Dispose();//释放资源
            //原始图片的宽、高
            int initWidth = initImage.Width;
            int initHeight = initImage.Height;

            //非正方型先裁剪为正方型
            if (initWidth != initHeight)
            {
                //截图对象
                System.Drawing.Image pickedImage = null;
                System.Drawing.Graphics pickedG = null;

                //宽大于高的横图
                if (initWidth > initHeight)
                {
                    //对象实例化
                    pickedImage = new System.Drawing.Bitmap(initHeight, initHeight);
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //定位
                    Rectangle fromR = new Rectangle((initWidth - initHeight) / 2, 0, initHeight, initHeight);
                    Rectangle toR = new Rectangle(0, 0, initHeight, initHeight);
                    //画图
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                    //重置宽
                    initWidth = initHeight;
                }
                //高大于宽的竖图
                else
                {
                    //对象实例化
                    pickedImage = new System.Drawing.Bitmap(initWidth, initWidth);
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);
                    //设置质量
                    pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                    //定位
                    Rectangle fromR = new Rectangle(0, (initHeight - initWidth) / 2, initWidth, initWidth);
                    Rectangle toR = new Rectangle(0, 0, initWidth, initWidth);
                    //画图
                    pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
                    //重置高
                    initHeight = initWidth;
                }

                //将截图对象赋给原图
                initImage = (System.Drawing.Image)pickedImage.Clone();
                //释放截图资源
                pickedG.Dispose();
                pickedImage.Dispose();
            }

            //缩略图对象
            System.Drawing.Image resultImage = new System.Drawing.Bitmap(side, side);
            System.Drawing.Graphics resultG = System.Drawing.Graphics.FromImage(resultImage);
            //设置质量
            resultG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            resultG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //用指定背景色清空画布
            resultG.Clear(Color.White);
            //绘制缩略图
            resultG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, side, side), new System.Drawing.Rectangle(0, 0, initWidth, initHeight), System.Drawing.GraphicsUnit.Pixel);

            //关键质量控制
            ImageCodecInfo ici = SunImgFormat.GetImageCodecInfo(imgExt);
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
            MemoryStream newMs = new MemoryStream();
            //保存缩略图
            resultImage.Save(newMs, ici, ep);

            //释放关键质量控制所用资源
            ep.Dispose();

            //释放缩略图资源
            resultG.Dispose();
            resultImage.Dispose();

            //释放原始图片资源
            initImage.Dispose();
            return newMs;
        }

        #endregion
        /// <summary>
        /// 指定位置裁剪图片
        /// </summary>
        /// <param name="imgStream">图片流</param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <param name="quality">质量</param>
        /// <param name="imgExt">图片后缀</param>
        /// <returns></returns>
        public static MemoryStream CutImg(System.IO.Stream imgStream, int x, int y, int x2, int y2, int quality, string imgExt)
        {
            //原始图片（获取原始图片创建对象，并使用流中嵌入的颜色管理信息）
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(imgStream, true);
            imgStream.Dispose();//释放资源
            int width = x2 - x;
            int height = y2 - y;
            //截图对象
            System.Drawing.Image pickedImage = new System.Drawing.Bitmap(width, height);
            System.Drawing.Graphics pickedG = System.Drawing.Graphics.FromImage(pickedImage);
            //设置质量
            pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            pickedG.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //定位
            Rectangle fromR = new Rectangle(x, y, width, height);
            Rectangle toR = new Rectangle(0, 0, width, height);
            //画图
            pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);
            //关键质量控制
            ImageCodecInfo ici = SunImgFormat.GetImageCodecInfo(imgExt);
            EncoderParameters ep = new EncoderParameters(1);
            ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
            MemoryStream newMs = new MemoryStream();
            //保存缩略图
            pickedImage.Save(newMs, ici, ep);

            //释放关键质量控制所用资源
            ep.Dispose();
            //释放原始图片资源
            initImage.Dispose();
            //释放截图资源
            pickedG.Dispose();
            pickedImage.Dispose();
            return newMs;
        }
        #region 自定义裁剪并缩放

        /// <summary>
        /// 指定长宽裁剪缩放
        /// 按模版比例最大范围的裁剪图片并缩放至模版尺寸
        /// </summary>
        /// <param name="fromFile">原图Stream对象</param>
        /// <param name="maxWidth">最大宽(单位:px)</param>
        /// <param name="maxHeight">最大高(单位:px)</param>
        /// <param name="quality">质量（范围0-100）</param>
        ///<param name="imgExt">图片后缀</param>
        public static MemoryStream CutForCustom(System.IO.Stream fromFile, int maxWidth, int maxHeight, int quality, string imgExt)
        {
            //从文件获取原始图片，并使用流中嵌入的颜色管理信息
            System.Drawing.Image initImage = System.Drawing.Image.FromStream(fromFile, true);
            ImageFormat tFormat = initImage.RawFormat;
            MemoryStream newMS = new MemoryStream();
            fromFile.Dispose();//释放资源
            //模版的宽高比例
            double templateRate = (double)maxWidth / maxHeight;
            //原图片的宽高比例
            double initRate = (double)initImage.Width / initImage.Height;

            //原图与模版比例相等，直接缩放
            if (templateRate == initRate)
            {
                //按模版大小生成最终图片
                System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                templateG.Clear(Color.White);
                templateG.DrawImage(initImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, initImage.Width, initImage.Height), System.Drawing.GraphicsUnit.Pixel);
                templateImage.Save(newMS, tFormat);
                return newMS;
            }
            //原图与模版比例不等，裁剪后缩放
            else
            {
                //裁剪对象
                System.Drawing.Image pickedImage = null;
                System.Drawing.Graphics pickedG = null;

                //定位
                Rectangle fromR = new Rectangle(0, 0, 0, 0);//原图裁剪定位
                Rectangle toR = new Rectangle(0, 0, 0, 0);//目标定位

                //宽为标准进行裁剪
                if (templateRate > initRate)
                {
                    //裁剪对象实例化
                    pickedImage = new System.Drawing.Bitmap(initImage.Width, (int)System.Math.Floor(initImage.Width / templateRate));
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                    //裁剪源定位
                    fromR.X = 0;
                    fromR.Y = (int)System.Math.Floor((initImage.Height - initImage.Width / templateRate) / 2);
                    fromR.Width = initImage.Width;
                    fromR.Height = (int)System.Math.Floor(initImage.Width / templateRate);

                    //裁剪目标定位
                    toR.X = 0;
                    toR.Y = 0;
                    toR.Width = initImage.Width;
                    toR.Height = (int)System.Math.Floor(initImage.Width / templateRate);
                }
                //高为标准进行裁剪
                else
                {
                    pickedImage = new System.Drawing.Bitmap((int)System.Math.Floor(initImage.Height * templateRate), initImage.Height);
                    pickedG = System.Drawing.Graphics.FromImage(pickedImage);

                    fromR.X = (int)System.Math.Floor((initImage.Width - initImage.Height * templateRate) / 2);
                    fromR.Y = 0;
                    fromR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                    fromR.Height = initImage.Height;

                    toR.X = 0;
                    toR.Y = 0;
                    toR.Width = (int)System.Math.Floor(initImage.Height * templateRate);
                    toR.Height = initImage.Height;
                }

                //设置质量
                pickedG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                pickedG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

                //裁剪
                pickedG.DrawImage(initImage, toR, fromR, System.Drawing.GraphicsUnit.Pixel);

                //按模版大小生成最终图片
                System.Drawing.Image templateImage = new System.Drawing.Bitmap(maxWidth, maxHeight);
                System.Drawing.Graphics templateG = System.Drawing.Graphics.FromImage(templateImage);
                templateG.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
                templateG.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                templateG.Clear(Color.White);
                templateG.DrawImage(pickedImage, new System.Drawing.Rectangle(0, 0, maxWidth, maxHeight), new System.Drawing.Rectangle(0, 0, pickedImage.Width, pickedImage.Height), System.Drawing.GraphicsUnit.Pixel);

                //关键质量控制
                //获取系统编码类型数组,包含了jpeg,bmp,png,gif,tiff
                ImageCodecInfo ici =SunImgFormat.GetImageCodecInfo(imgExt);
                EncoderParameters ep = new EncoderParameters(1);
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)quality);
                //保存缩略图
                templateImage.Save(newMS, ici, ep);
                //templateImage.Save(fileSaveUrl, System.Drawing.Imaging.ImageFormat.Jpeg);

                //释放资源
                ep.Dispose();
                templateG.Dispose();
                templateImage.Dispose();

                pickedG.Dispose();
                pickedImage.Dispose();
            }

            //释放资源
            initImage.Dispose();
            return newMS;
        }
        #endregion

        /// <summary>
        /// 按原图片比例获取缩略图
        /// </summary>
        /// <param name="sourceImg"></param>
        /// <param name="Width"></param>
        /// <param name="quality"></param>
        ///<param name="imgExt">图片后缀</param>
        /// <returns></returns>
        public static MemoryStream GetImgThumbnail(System.IO.Stream sourceImg, int Width, int quality,string imgExt)
        {

            System.Drawing.Image iSource = System.Drawing.Image.FromStream(sourceImg);
            sourceImg.Dispose();
            MemoryStream newMs = new MemoryStream();
            ImageFormat tFormat = iSource.RawFormat;
            int sW = 0, sH = 0;
            double srate = iSource.Width/(iSource.Height*1.0);//原图宽高比例
            int Height = Convert.ToInt32(Math.Floor(Width / srate));
            //按比例缩放
            Size tem_size = new Size(iSource.Width, iSource.Height);

            if (tem_size.Width > Height || tem_size.Width > Width)
            {
                if ((tem_size.Width * Height) > (tem_size.Height * Width))
                {
                    sW = Width;
                    sH = (Width * tem_size.Height) / tem_size.Width;
                }
                else
                {
                    sH = Height;
                    sW = (tem_size.Width * Height) / tem_size.Height;
                }
            }
            else
            {
                sW = tem_size.Width;
                sH = tem_size.Height;
            }
            Bitmap ob = new Bitmap(Width, Height);
            Graphics g = Graphics.FromImage(ob);
            g.Clear(Color.WhiteSmoke);
            g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(iSource, new Rectangle((Width - sW) / 2-1, (Height - sH) / 2-1, sW+2, sH+1), 0, 0, iSource.Width, iSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo jpegICIinfo = SunImgFormat.GetImageCodecInfo(imgExt);
                if (jpegICIinfo != null)
                {
                    ob.Save(newMs, jpegICIinfo, ep);
                }
                else
                {
                    ob.Save(newMs, tFormat);
                }
                return newMs;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
                ep.Dispose();
            }

        }
        /// <summary>
        /// 压缩图片质量
        /// </summary>
        /// <param name="sourceImg">图片流</param>
        /// <param name="quality">图片质量</param>
        ///<param name="imgExt">图片后缀</param>
        /// <returns></returns>
        public static MemoryStream ImgReduce(System.IO.Stream sourceImg,int quality,string imgExt)
        {
            System.Drawing.Image iSource = System.Drawing.Image.FromStream(sourceImg);
            sourceImg.Dispose();
            MemoryStream newMs = new MemoryStream();
            ImageFormat tFormat = iSource.RawFormat;

            Bitmap ob = new Bitmap(iSource);;
            //以下代码为保存图片时，设置压缩质量
            EncoderParameters ep = new EncoderParameters();
            EncoderParameter eParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality);
            ep.Param[0] = eParam;
            try
            {
                ImageCodecInfo codeinfo = SunImgFormat.GetImageCodecInfo(imgExt);
                if (codeinfo != null)
                {
                    ob.Save(newMs, codeinfo, ep);
                }
                else
                {
                    ob.Save(newMs, tFormat);
                }
                return newMs;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                iSource.Dispose();
                ob.Dispose();
                ep.Dispose();
            }

        }
    }//end class
}