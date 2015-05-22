using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Sun.Framework.Calculate;
using System.Drawing.Drawing2D;

namespace Sun.Framework.SunImg
{
    public class CaptchaCode
    {
        /// <summary>
        /// 字体颜色
        /// </summary>
        static Color[] colors = { Color.Black, Color.FromArgb(255, 130, 255), Color.FromArgb(53, 154, 255), Color.DarkBlue, Color.Green, Color.Brown, Color.FromArgb(255, 149, 149), Color.CornflowerBlue, Color.DarkCyan, Color.Purple, Color.DarkOliveGreen, Color.FromArgb(176, 98, 255),Color.FromArgb(0,120,240),Color.FromArgb(128,0,255),Color.FromArgb(128,64,0),Color.FromArgb(128,128,64),Color.FromArgb(108,108,183) };
        /// <summary>
        /// 背景图片色
        /// </summary>
        static Color[] backColors = {Color.FromArgb(255, 236, 255), Color.FromArgb(255, 255, 198), Color.FromArgb(230, 204, 255), Color.FromArgb(224, 193, 255), Color.FromArgb(255, 223, 223), Color.FromArgb(255, 214, 172), Color.FromArgb(216, 235, 235), Color.FromArgb(215, 255, 255), Color.FromArgb(227, 227, 255), Color.FromArgb(239, 255, 223), Color.FromArgb(206, 231, 255) };

        //定义字体 
        static string[] fonts = { "Verdana", "Microsoft Sans Serif", "Comic Sans MS", "Arial", "宋体", "仿宋", "新宋体", "楷体", "黑体" };
        /// <summary>
        /// 生成随机中文字符图片
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static CaptchaInfo DrawChinaImage(int Length)
        {
            string checkCode = RandomHelper.CreateChinaCode(Length);
            CaptchaInfo cap = new CaptchaInfo();
            cap.Result = checkCode;
            cap.ImgData = CreateImage(checkCode);
            return cap;
        }
        /// <summary>
        /// 生成随机九宫格中文字符验证码
        /// </summary>
        /// <returns></returns>
        public static CaptchaInfo DrawChinaSudokuImage()
        {
            string checkCode = RandomHelper.CreateChinaCode(9);
            string str = null;
            int count = 3;
            int[] list = new int[4];
            Random rd = new Random();
            while (count >= 0)
            {
                var v = rd.Next(9);
                if (!list.Contains(v))
                {
                    list[count] = v;
                    str += v;
                    count--;
                }
            }
            CaptchaInfo cap = new CaptchaInfo();
            cap.Result = str;
            cap.ImgData = CreateSudokuImage(checkCode, list);
            return cap;
        }
        /// <summary>
        /// 生成数字字母验证码图片
        /// </summary>
        /// <param name="length">生成认证长度</param>
        public static CaptchaInfo DrawNumberImage(int length)
        {
            string checkCode = RandomHelper.Str(length);
            CaptchaInfo cap = new CaptchaInfo();
            cap.Result = checkCode;
            cap.ImgData = CreateImage(checkCode);
            return cap;
        }
        /// <summary>
        /// 创建随机码图片
        /// </summary>
        /// <param name="SunRandomcode">随机码</param>
        private static byte[] CreateImage(string SunRandomcode)
        {
            int randAngle = 30; //随机转动角度
            //定义颜色


            System.Random rand = new System.Random(unchecked((int)System.DateTime.Now.Ticks));

            int mapwidth = (int)(SunRandomcode.Length * 22);
            int height = 40;
            Bitmap map = new Bitmap(mapwidth, height);//创建图片背景
            Graphics graph = Graphics.FromImage(map);
            graph.SmoothingMode = SmoothingMode.AntiAlias; //使绘图质量最高，即消除锯齿
            graph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graph.CompositingQuality = CompositingQuality.HighQuality;
            graph.Clear(Color.White);//
            RectangleF rectangle = new RectangleF(new Point(0, 0), new SizeF(mapwidth, height));
            Brush brush = new LinearGradientBrush(rectangle, backColors[rand.Next(backColors.Count())], backColors[rand.Next(backColors.Count())], LinearGradientMode.Horizontal);//绘制背景
            graph.FillRectangle(brush, rectangle);
            graph.DrawRectangle(new Pen(Color.Black, 0), 0, 0, map.Width - 1, map.Height - 1);//画一个边框
            graph.DrawBezier(new Pen(colors[rand.Next(12)], 2.0f), new Point(2, rand.Next(height)), new Point(rand.Next(mapwidth), rand.Next(height)), new Point(rand.Next(mapwidth), rand.Next(height)), new Point(mapwidth, rand.Next(height)));
           // graph.DrawBezier(new Pen(colors[rand.Next(12)], 2.0f), new Point(2, rand.Next(height)), new Point(rand.Next(mapwidth), rand.Next(height)), new Point(rand.Next(mapwidth), rand.Next(height)), new Point(mapwidth, rand.Next(height)));

            //验证码旋转，防止机器识别
            char[] chars = SunRandomcode.ToCharArray();//拆散字符串成单字符数组
            //文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < chars.Length; i++)
            {
                int cindex = rand.Next(colors.Count());
                int findex = rand.Next(fonts.Count());
                Font f = new System.Drawing.Font(fonts[findex], 25, FontStyle.Bold);//字体样式(参数2为字体大小)
                Brush b = new System.Drawing.SolidBrush(colors[cindex]);
                Point dot = new Point(16, 18);//字符间距设置
                float angle = rand.Next(-randAngle, randAngle);//转动的度数
                graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                graph.RotateTransform(-angle);//转回去
                graph.TranslateTransform(2, -dot.Y);//移动光标到指定位置
            }

            //生成图片
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            graph.Dispose();
            map.Dispose();
            return ms.ToArray();
        }
        /// <summary>
        /// 创建九宫格随机码图片
        /// </summary>
        /// <param name="charsStr">字符串</param>
        /// <param name="charsIndex">正确字符索引</param>
        /// <returns>图片二进制</returns>
        private static byte[] CreateSudokuImage(string charsStr, int[] charsIndex)
        {
            int topRandAngle = 40;
            int randAngle = 30; //随机转动角度
            System.Random rand = new System.Random();
            int mapwidth = 145;
            int height = 180;
            int topHeight = 40;
            Bitmap map = new Bitmap(mapwidth, height);//创建图片背景
            Graphics graph = Graphics.FromImage(map);
            graph.Clear(Color.White);
            graph.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            graph.DrawBezier(new Pen(colors[rand.Next(12)], 1.5f), new Point(2, rand.Next(topHeight)), new Point(rand.Next(mapwidth), rand.Next(topHeight)), new Point(rand.Next(mapwidth), rand.Next(topHeight)), new Point(mapwidth, rand.Next(topHeight)));
            graph.DrawBezier(new Pen(colors[rand.Next(12)], 1.5f), new Point(2, rand.Next(topHeight)), new Point(rand.Next(mapwidth), rand.Next(topHeight)), new Point(rand.Next(mapwidth), rand.Next(topHeight)), new Point(mapwidth, rand.Next(topHeight)));

            //验证码旋转，防止机器识别
            char[] chars = charsStr.ToCharArray();//拆散字符串成单字符数组
            //文字距中
            StringFormat format = new StringFormat(StringFormatFlags.NoClip);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            var fontColor = colors[rand.Next(colors.Count())];
            Font f = new System.Drawing.Font("楷体", 18);//字体样式(参数2为字体大小)
            for (int i = 0; i < 4; i++)
            {
                Brush b = new System.Drawing.SolidBrush(fontColor);
                var y = i > 0 ? 25 : 20;
                Point dot = new Point(27, y);
                float angle = rand.Next(-topRandAngle, topRandAngle);//转动的度数
                graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                graph.RotateTransform(angle);
                graph.DrawString(chars[charsIndex[i]].ToString(), f, b, 1, 1, format);
                graph.RotateTransform(-angle);//转回去
                graph.TranslateTransform(2, -dot.Y);//移动光标到指定位置
            }
            graph.TranslateTransform(-128, 45);//移动光标到指定位置
            for (int i = 0; i < 9; i++)
            {
                Brush b = new System.Drawing.SolidBrush(fontColor);
                Point dot = new Point(40, 25);
                float angle = rand.Next(-randAngle, randAngle);//转动的度数
                graph.TranslateTransform(dot.X, dot.Y);//移动光标到指定位置
                graph.RotateTransform(angle);
                graph.DrawString(chars[i].ToString(), f, b, 1, 1, format);
                graph.RotateTransform(-angle);//转回去
                graph.TranslateTransform(5, -dot.Y);//移动光标到指定位置
                if (i == 2 || i == 5)
                    graph.TranslateTransform(-135, 45);//移动光标到指定位置
            }
            //生成图片
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            map.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            graph.Dispose();
            map.Dispose();
            return ms.ToArray();
        }
    }
}
