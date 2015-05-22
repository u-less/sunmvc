using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Sun.Framework.SunImg;
using Sun.Core.Upload;

namespace Sun.Framework.Upload
{
    /// <summary>
    /// 文件本地上传接口
    /// </summary>
    public class FileUpload:IUpload
    {
        public FileUpload()
        {
            var context = HttpContext.Current;
            this.Request = context.Request;
            this.Server = context.Server;
        }

    public HttpRequest Request { get; private set; }
    public HttpServerUtility Server { get; private set; }
    public UploadResult Upload(UploadConfig config)
    {
        UploadResult Result = new UploadResult() { State = UploadState.Unknown };
        byte[] uploadFileBytes = null;
        string uploadFileName = null;
        string fileExt = null;
        if (config.Base64)
        {
            uploadFileName = config.Base64Filename;
            fileExt = ".png";
            uploadFileBytes = Convert.FromBase64String(Request[config.UploadFieldName]);
        }
        else
        {
            var file = Request.Files[0];
            uploadFileName = file.FileName;
            fileExt = Path.GetExtension(uploadFileName).ToLower();
            if (!config.AllowExtensions.Contains(fileExt))
            {
                Result.State = UploadState.TypeNotAllow;
                return Result;
            }
            if (file.ContentLength > config.SizeLimit)
            {
                Result.State = UploadState.SizeLimitExceed;
                return Result;
            }

            uploadFileBytes = new byte[file.ContentLength];
            try
            {
                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;
                return Result;
            }
        }

        Result.OriginFileName = uploadFileName;

        var savePath = config.PathFormat + DateTime.Now.ToString("yyyy/MM/");
        savePath += Guid.NewGuid() + fileExt;
        var localPath = Server.MapPath(savePath);
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(localPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            }
            File.WriteAllBytes(localPath, uploadFileBytes);
            Result.Url = savePath;
            Result.State = UploadState.Success;
        }
        catch (Exception e)
        {
            Result.State = UploadState.FileAccessError;
            Result.ErrorMessage = e.Message;
        }
        return Result;
    }
     /// <summary>
     /// 使用参数上传图片
     /// </summary>
     /// <param name="config">配置</param>
     /// <param name="defaultQuality">默认图片质量（1-100）</param>
     /// <param name="t">图片处理方式</param>
     /// <param name="width">宽</param>
     /// <param name="height">高</param>
     /// <returns></returns>
    public UploadResult UseAttrUploadImage(UploadConfig config, int defaultQuality, CutImageType t = CutImageType.Original, int width = 0, int height = 0)
    {
        UploadResult Result = new UploadResult() { State = UploadState.Unknown };
        byte[] uploadFileBytes = null;
        string uploadFileName = null;
        string fileExt = null;
        if (config.Base64)
        {
            uploadFileName = config.Base64Filename;
            fileExt = ".png";
            uploadFileBytes = Convert.FromBase64String(Request[config.UploadFieldName]);
        }
        else
        {
            var file = Request.Files[0];
            uploadFileName = file.FileName;
            fileExt = Path.GetExtension(uploadFileName).ToLower();
            if (!config.AllowExtensions.Contains(fileExt))
            {
                Result.State = UploadState.TypeNotAllow;
                return Result;
            }
            if (file.ContentLength > config.SizeLimit)
            {
                Result.State = UploadState.SizeLimitExceed;
                return Result;
            }
            try
            {
                int q = file.ContentLength/1024/1024 > 0.5 ? defaultQuality : 100;
                switch (t)
                {
                    case CutImageType.Thumbnail:
                        {
                            uploadFileBytes = SunImage.GetImgThumbnail(file.InputStream, width, q, fileExt).GetBuffer();
                        }; break;
                    case CutImageType.CutForCustom:
                        {
                            uploadFileBytes = SunImage.CutForCustom(file.InputStream, width, height, q, fileExt).GetBuffer();
                        }; break;
                    case CutImageType.CutForSquare:
                        {
                            uploadFileBytes = SunImage.CutForSquare(file.InputStream, width, q, fileExt).GetBuffer();
                        }; break;
                    default:
                        {
                            if (q != 100)
                                uploadFileBytes = SunImage.ImgReduce(file.InputStream, q, fileExt).GetBuffer();
                            else
                            {
                                uploadFileBytes = new byte[file.ContentLength];
                                file.InputStream.Read(uploadFileBytes, 0, file.ContentLength);
                            }
                        }; break;
                }
            }
            catch (Exception)
            {
                Result.State = UploadState.NetworkError;
                return Result;
            }
        }
        Result.OriginFileName = uploadFileName;

        var savePath = config.PathFormat + DateTime.Now.ToString("yyyy/MM/");
        savePath += Guid.NewGuid() + fileExt;
        var localPath = Server.MapPath(savePath);
        try
        {
            if (!Directory.Exists(Path.GetDirectoryName(localPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(localPath));
            }
            File.WriteAllBytes(localPath, uploadFileBytes);
            Result.Url = savePath;
            Result.State = UploadState.Success;
        }
        catch (Exception e)
        {
            Result.State = UploadState.FileAccessError;
            Result.ErrorMessage = e.Message;
        }
        return Result;
    }
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GetStateMessage(UploadState state)
        {
            switch (state)
            {
                case UploadState.Success:
                    return "SUCCESS";
                case UploadState.FileAccessError:
                    return "权限错误";
                case UploadState.SizeLimitExceed:
                    return "文件大小超出服务器限制";
                case UploadState.TypeNotAllow:
                    return "不允许的文件格式";
                case UploadState.NetworkError:
                    return "网络错误";
            }
            return "未知错误";
        }
        IEnumerable<string> IUpload.GetFiles(string collection, string ext, int start, int size)
        {
            return new DirectoryInfo(Server.MapPath(collection)).GetFiles().OrderByDescending(t => t.CreationTime).Skip(start).Take(size).Select(f=>f.Name);
        }
    }
}
