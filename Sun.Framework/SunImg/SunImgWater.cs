using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace Sun.Framework.SunImg
{
    public class SunImgWater
    {
        /// <summary>
        /// 加图片水印
        /// </summary>
        /// <param name="sourceImgMS">要加水印的原图﻿(System.Drawing)</param>
        /// <param name="watermarkImg">水印文件名</param>
        /// <param name="watermarkStatus">图片水印位置1=左上 2=中上 3=右上 4=左中  5=中中 6=右中 7=左下 8=右中 9=右下</param>
        /// <param name="quality">加水印后的质量0~100,数字越大质量越高</param>
        /// <param name="watermarkTransparency">水印图片的透明度1~10,数字越小越透明,10为不透明</param>
        ///<param name="imgExt">图片后缀</param>
        public static MemoryStream ImageWaterMarkPic(MemoryStream sourceImgMS, MemoryStream watermarkImg, int watermarkStatus, int quality, int watermarkTransparency,string imgExt)
        {
            Image sourceImg = Image.FromStream(sourceImgMS);//原图
            Image watermark = Image.FromStream(watermarkImg);//水印图
            ImageFormat tFormat = sourceImg.RawFormat;//获取原图格式
            Graphics g = Graphics.FromImage(sourceImg);
            //设置高质量插值法
            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            //设置高质量,低速度呈现平滑程度
            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
           
            if (watermark.Height >= sourceImg.Height || watermark.Width >= sourceImg.Width)
                throw new Exception("水印图片大于原图");

            ImageAttributes imageAttributes = new ImageAttributes();
            ColorMap colorMap = new ColorMap();

            colorMap.OldColor = Color.FromArgb(255, 0, 255, 0);
            colorMap.NewColor = Color.FromArgb(0, 0, 0, 0);
            ColorMap[] remapTable = { colorMap };

            imageAttributes.SetRemapTable(remapTable, ColorAdjustType.Bitmap);

            float transparency = 0.5F;
            if (watermarkTransparency >= 1 && watermarkTransparency <= 10)
                transparency = (watermarkTransparency / 10.0F);


            float[][] colorMatrixElements = {
                                                new float[] {1.0f,  0.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  1.0f,  0.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  1.0f,  0.0f, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  transparency, 0.0f},
                                                new float[] {0.0f,  0.0f,  0.0f,  0.0f, 1.0f}
                                            };

            ColorMatrix colorMatrix = new ColorMatrix(colorMatrixElements);

            imageAttributes.SetColorMatrix(colorMatrix, ColorMatrixFlag.Default, ColorAdjustType.Bitmap);

            int xpos = 0;
            int ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (int)(sourceImg.Width * (float).01);
                    ypos = (int)(sourceImg.Height * (float).01);
                    break;
                case 2:
                    xpos = (int)((sourceImg.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)(sourceImg.Height * (float).01);
                    break;
                case 3:
                    xpos = (int)((sourceImg.Width * (float).99) - (watermark.Width));
                    ypos = (int)(sourceImg.Height * (float).01);
                    break;
                case 4:
                    xpos = (int)(sourceImg.Width * (float).01);
                    ypos = (int)((sourceImg.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 5:
                    xpos = (int)((sourceImg.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((sourceImg.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 6:
                    xpos = (int)((sourceImg.Width * (float).99) - (watermark.Width));
                    ypos = (int)((sourceImg.Height * (float).50) - (watermark.Height / 2));
                    break;
                case 7:
                    xpos = (int)(sourceImg.Width * (float).01);
                    ypos = (int)((sourceImg.Height * (float).99) - watermark.Height);
                    break;
                case 8:
                    xpos = (int)((sourceImg.Width * (float).50) - (watermark.Width / 2));
                    ypos = (int)((sourceImg.Height * (float).99) - watermark.Height);
                    break;
                case 9:
                    xpos = (int)((sourceImg.Width * (float).99) - (watermark.Width));
                    ypos = (int)((sourceImg.Height * (float).99) - watermark.Height);
                    break;
            }

            g.DrawImage(watermark, new Rectangle(xpos, ypos, watermark.Width, watermark.Height), 0, 0, watermark.Width, watermark.Height, GraphicsUnit.Pixel, imageAttributes);

            ImageCodecInfo ici = SunImgFormat.GetImageCodecInfo(imgExt);
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;
            MemoryStream newMS = new MemoryStream();
            if (ici != null)
                sourceImg.Save(newMS, ici, encoderParams);
            else
                sourceImg.Save(newMS,tFormat);

            watermarkImg.Dispose();//释放资源
            sourceImgMS.Dispose();//释放资源
            g.Dispose();
            sourceImg.Dispose();
            watermark.Dispose();
            imageAttributes.Dispose();
            return newMS;
        }

        /// <summary>
        /// 增加图片文字水印
        /// </summary>
        /// <param name="sourceImgMS">要加水印的原图﻿(﻿System.Drawing)</param>
        /// <param name="watermarkText">水印文字</param>
        /// <param name="watermarkStatus">图片水印位置1=左上 2=中上 3=右上 4=左中  5=中中 6=右中 7=左下 8=右中 9=右下</param>
        /// <param name="quality">加水印后的质量0~100,数字越大质量越高</param>
        /// <param name="fontname">水印的字体</param>
        /// <param name="fontsize">水印的字号</param>
        ///<param name="imgExt">图片后缀</param>
        public static MemoryStream ImageWaterMarkText(MemoryStream sourceImgMS, string watermarkText, int watermarkStatus, int quality, string fontname, int fontsize,Color color,string imgExt)
        {
            Image img = Image.FromStream(sourceImgMS);
            ImageFormat sFormat = img.RawFormat;//获取原图格式
            Graphics g = Graphics.FromImage(img);
            Font drawFont = new Font(fontname, fontsize, FontStyle.Regular, GraphicsUnit.Pixel);
            SizeF crSize;
            crSize = g.MeasureString(watermarkText, drawFont);

            float xpos = 0;
            float ypos = 0;

            switch (watermarkStatus)
            {
                case 1:
                    xpos = (float)img.Width * (float).01;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 2:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = (float)img.Height * (float).01;
                    break;
                case 3:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = (float)img.Height * (float).01;
                    break;
                case 4:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 5:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 6:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).50) - (crSize.Height / 2);
                    break;
                case 7:
                    xpos = (float)img.Width * (float).01;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 8:
                    xpos = ((float)img.Width * (float).50) - (crSize.Width / 2);
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
                case 9:
                    xpos = ((float)img.Width * (float).99) - crSize.Width;
                    ypos = ((float)img.Height * (float).99) - crSize.Height;
                    break;
            }
            //g.DrawString(watermarkText, drawFont, new SolidBrush(Color.White), xpos + 1, ypos + 1);文字阴影
            g.DrawString(watermarkText, drawFont, new SolidBrush(color), xpos, ypos);
            ImageCodecInfo ici = SunImgFormat.GetImageCodecInfo(imgExt);
            EncoderParameters encoderParams = new EncoderParameters();
            long[] qualityParam = new long[1];
            if (quality < 0 || quality > 100)
                quality = 80;

            qualityParam[0] = quality;

            EncoderParameter encoderParam = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, qualityParam);
            encoderParams.Param[0] = encoderParam;
            MemoryStream newMs = new MemoryStream();
            if (ici != null)
                img.Save(newMs, ici, encoderParams);
            else
                img.Save(newMs,sFormat);
            sourceImgMS.Dispose();
            g.Dispose();
            img.Dispose();
            return newMs;
        }
    }
}