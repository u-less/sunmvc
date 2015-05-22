using System;

namespace Plugin.MongoUpload
{
    /// <summary>
    /// 图片
    /// </summary>
    public class SunFile
    {
        /// <summary>
        /// 唯一编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 上传时间
        /// </summary>
        public DateTime Time { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public byte[] Data { get; set; }
    }
}
