using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Upload
{
    /// <summary>
    /// 上传状态
    /// </summary>
    public enum UploadState
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 0,
        /// <summary>
        /// 文件大小超出限制
        /// </summary>
        SizeLimitExceed = -1,
        /// <summary>
        /// 类型错误
        /// </summary>
        TypeNotAllow = -2,
        /// <summary>
        /// 没有权限
        /// </summary>
        FileAccessError = -3,
        /// <summary>
        /// 网络错误
        /// </summary>
        NetworkError = -4,
        /// <summary>
        /// 未知错误
        /// </summary>
        Unknown = 1
    }
}
