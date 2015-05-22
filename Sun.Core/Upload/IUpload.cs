using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Upload
{
    public interface IUpload
    {
        /// <summary>
        /// 上传文件(含图片)
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        UploadResult Upload(UploadConfig config);
        /// <summary>
        /// 使用参数上传图片
        /// </summary>
        /// <param name="config"></param>
        /// <param name="defaultQuality">图片质量</param>
        /// <param name="t">处理方法</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns></returns>
        UploadResult UseAttrUploadImage(UploadConfig config,int defaultQuality, CutImageType t = CutImageType.Original, int width = 0, int height = 0);
         /// <summary>
        /// 获取状态信息
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        string GetStateMessage(UploadState state);
        /// <summary>
        /// 获取文件地址列表
        /// </summary>
        /// <param name="collection">文件集合</param>
        /// <param name="ext">文件后缀</param>
        /// <param name="start">开始位置</param>
        /// <param name="size">文件数量</param>
        /// <returns>文件分页数据</returns>
        IEnumerable<string> GetFiles(string collection, string ext, int start, int size);
    }
}
