using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Upload
{
    /// <summary>
    /// 图片上传结果
    /// </summary>
    public class UploadResult
    {
        /// <summary>
        /// 图片Id
        /// </summary>
        public string FileId
        {
            get;
            set;
        }
        /// <summary>
        /// 状态
        /// </summary>
        public UploadState State { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 图片名称
        /// </summary>
        public string OriginFileName { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>

        public string ErrorMessage { get; set; }
    }
}
