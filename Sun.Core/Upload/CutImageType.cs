using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Upload
{
    public enum CutImageType
    {
        /// <summary>
        /// 原图
        /// </summary>
        Original = 0,
        /// <summary>
        /// 以图片中心为轴心，截取正方型
        /// </summary>
        CutForSquare = 1,
        /// <summary>
        /// 以图片中心为轴心，指定长宽裁剪
        /// </summary>
        CutForCustom = 2,
        /// <summary>
        /// 按原图片比例获取缩略图
        /// </summary>
        Thumbnail = 3
    }
}
