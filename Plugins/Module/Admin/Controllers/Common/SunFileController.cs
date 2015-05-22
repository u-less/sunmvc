using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using Sun.Core.Upload;
using Sun.Model.DB;
using Sun.Framework.SunImg;
using Sun.BaseOperate.Interface;
using Sun.BaseOperate.Config;
using Sun.Core.Ioc;
using Autofac;

namespace Plugin.Admin.Controllers.Common
{
    public class SunFileController  : Controller
    {
        [Route("sunfile/upload/{collection}/{ac?}/")]
        public JsonResult Upload(string collection=null,string ac=null)
        {
            IUpload UpOp=WebIoc.Container.Resolve<IUpload>();
            var context = ControllerContext.HttpContext;
            if(string.IsNullOrEmpty(ac))
                ac=context.Request["action"];
            switch (ac)
            {
                case "config": return UploadConfig();
                case "uploadimage":
                    {
                        UploadResult result = UpOp.Upload(new UploadConfig()
                        {
                            AllowExtensions = CustomConfig.ImgExts.ToArray(),
                            PathFormat = CustomConfig.EditorImgPath,
                            SizeLimit = Convert.ToInt32(CustomConfig.CanUploadImgSize * 1024 * 1024),
                            UploadFieldName = "upfile"
                        });
                        return Json(new
                        {
                            fileid=result.FileId,
                            state = UpOp.GetStateMessage(result.State),
                            url = result.Url,
                            title = result.OriginFileName,
                            original = result.OriginFileName,
                            error = result.ErrorMessage
                        }, "text/html");
                    }
                case "uploadscrawl":
                    {
                        UploadResult result = UpOp.Upload(new UploadConfig()
                        {
                            AllowExtensions = CustomConfig.ImgExts.ToArray(),
                            PathFormat = CustomConfig.EditorImgPath,
                            SizeLimit = Convert.ToInt32(CustomConfig.CanUploadImgSize * 1024 * 1024),
                            UploadFieldName = "upfile",
                            Base64 = true,
                            Base64Filename = "scrawl.png"
                        });
                        return Json(new
                        {
                            fileid = result.FileId,
                            state = UpOp.GetStateMessage(result.State),
                            url = result.Url,
                            title = result.OriginFileName,
                            original = result.OriginFileName,
                            error = result.ErrorMessage
                        }, "text/html");
                    }
                case "uploadvideo":
                    {
                        UploadResult result = UpOp.Upload(new UploadConfig()
                        {
                            AllowExtensions = CustomConfig.VideoExts.ToArray(),
                            PathFormat = CustomConfig.EditorFilePath,
                            SizeLimit = Convert.ToInt32(CustomConfig.CanUploadFileSize * 1024 * 1024),
                            UploadFieldName = "upfile",
                        });
                        return Json(new
                        {
                            fileid = result.FileId,
                            state = UpOp.GetStateMessage(result.State),
                            url = result.Url,
                            title = result.OriginFileName,
                            original = result.OriginFileName,
                            error = result.ErrorMessage
                        }, "text/html");
                    }
                case "uploadfile":
                    {
                        UploadResult result = UpOp.Upload(new UploadConfig()
                        {
                            AllowExtensions = CustomConfig.FileExts.ToArray(),
                            PathFormat = CustomConfig.EditorFilePath,
                            SizeLimit = Convert.ToInt32(CustomConfig.CanUploadFileSize * 1024 * 1024),
                            UploadFieldName = "upfile",
                        });
                        return Json(new
                        {
                            fileid = result.FileId,
                            state = UpOp.GetStateMessage(result.State),
                            url = result.Url,
                            title = result.OriginFileName,
                            original = result.OriginFileName,
                            error = result.ErrorMessage
                        }, "text/html");
                    }
                case "listimage":
                    {
                        int Start = 0, Size = 20, Total = 300;
                        ResultState State=ResultState.Success;
                        IEnumerable<String> imgList = null;
                        Start = String.IsNullOrEmpty(Request["start"]) ? 1 : Convert.ToInt32(Request["start"]);
                        Size = String.IsNullOrEmpty(Request["size"]) ? 20 : Convert.ToInt32(Request["size"]);
                        try
                        {
                            var path = collection;
                            if (string.IsNullOrEmpty(collection))
                                path = CustomConfig.EditorImgPath + DateTime.Now.ToString("yyyy/MM/");
                            imgList = UpOp.GetFiles(path, "", Start, Size);
                            if (string.IsNullOrEmpty(collection))
                                imgList = imgList.Select(x => path + x);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            State = ResultState.AuthorizError;
                        }
                        catch (DirectoryNotFoundException)
                        {
                            State = ResultState.PathNotFound;
                        }
                        catch (IOException)
                        {
                            State = ResultState.IOError;
                        }
                        return Json(new
                        {
                            state = GetStateString(State),
                            list = imgList.Select(x => new { url = x }),
                            start = Start,
                            size = Size,
                            total = Total
                        }, JsonRequestBehavior.AllowGet);
                    }
                case "listfile":
                    {
                        int Start = 0, Size = 20, Total = 300;
                        ResultState State=ResultState.Success;
                        IEnumerable<String> fileList = null;
                        Start = String.IsNullOrEmpty(Request["start"]) ? 0 : Convert.ToInt32(Request["start"]);
                        Size = String.IsNullOrEmpty(Request["size"]) ? 20 : Convert.ToInt32(Request["size"]);
                        try
                        {
                            var path = collection;
                            if (string.IsNullOrEmpty(collection))
                                path = CustomConfig.EditorImgPath + DateTime.Now.ToString("yyyy/MM/");
                            fileList = UpOp.GetFiles(path, "", Start, Size);
                            if (string.IsNullOrEmpty(collection))
                                fileList = fileList.Select(x => path + x);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            State = ResultState.AuthorizError;
                        }
                        catch (DirectoryNotFoundException)
                        {
                            State = ResultState.PathNotFound;
                        }
                        catch (IOException)
                        {
                            State = ResultState.IOError;
                        }
                        return Json(new
                        {
                            state = GetStateString(State),
                            list = fileList.Select(x => new { url = x }),
                            start = Start,
                            size = Size,
                            total = 0
                        }, JsonRequestBehavior.AllowGet);
                    }
                case "catchimage":
                    {
                        string[] Sources;
                        CatchImg[] Crawlers;
                        Sources = Request.Form.GetValues("source[]");
                        if (Sources == null || Sources.Length == 0)
                        {
                            return Json(new { state = "参数错误：没有指定抓取源" });
                        } 
                        else
                            Crawlers = Sources.Select(x => new CatchImg(x, context.Server).Fetch(CustomConfig.EditorImgPath)).ToArray();
                        return Json(new
                        {
                            state = "SUCCESS",
                            list = Crawlers.Select(x => new
                            {
                                state = x.State,
                                source = x.SourceUrl,
                                url = x.ServerUrl
                            })
                        },JsonRequestBehavior.AllowGet);
                    }
                default:
                    return Json(new { state = "action 参数为空或者 action 不被支持。" });
            }
        }
        [OutputCache(Duration=3600)]
        private JsonResult UploadConfig()
        {
            return Json(new
                        {
                            imageActionName = "uploadimage",
                            imageFieldName = "upfile",
                            imageMaxSize = CustomConfig.CanUploadImgSize * 1024 * 1024,
                            imageAllowFiles = CustomConfig.ImgExts,
                            imageCompressEnable = true,
                            imageCompressBorder = 2000,
                            imageInsertAlign = "none",
                            imageUrlPrefix = "",
                            imagePathFormat = CustomConfig.EditorImgPath,
                            /*涂鸦配置*/
                            scrawlActionName = "uploadscrawl", /* 执行上传涂鸦的action名称 */
                            scrawlFieldName = "upfile", /* 提交的图片表单名称 */
                            scrawlPathFormat = CustomConfig.EditorImgPath, /* 上传保存路径,可以自定义保存路径和文件名格式 */
                            scrawlMaxSize = CustomConfig.CanUploadImgSize * 1024 * 1024, /* 上传大小限制，单位B */
                            scrawlUrlPrefix = "", /* 图片访问路径前缀 */
                            scrawlInsertAlign = "none",
                            /* 抓取远程图片配置 */
                            catcherLocalDomain = new string[] { "127.0.0.1", "localhost" },
                            catcherActionName = "catchimage", /* 执行抓取远程图片的action名称 */
                            catcherFieldName = "source", /* 提交的图片列表表单名称 */
                            catcherPathFormat = CustomConfig.EditorImgPath, /* 上传保存路径,可以自定义保存路径和文件名格式 */
                            catcherUrlPrefix = "", /* 图片访问路径前缀 */
                            catcherMaxSize = CustomConfig.CanUploadImgSize * 1024 * 1024, /* 上传大小限制，单位B */
                            catcherAllowFiles = CustomConfig.ImgExts, /* 抓取图片格式显示 */

                            /* 上传视频配置 */
                            videoActionName = "uploadvideo", /* 执行上传视频的action名称 */
                            videoFieldName = "upfile", /* 提交的视频表单名称 */
                            videoPathFormat = CustomConfig.EditorFilePath, /* 上传保存路径,可以自定义保存路径和文件名格式 */
                            videoUrlPrefix = "", /* 视频访问路径前缀 */
                            videoMaxSize = CustomConfig.CanUploadFileSize * 1024 * 1024, /* 上传大小限制，单位B，默认100MB */
                            videoAllowFiles = CustomConfig.VideoExts, /* 上传视频格式显示 */

                            /* 上传文件配置 */
                            fileActionName = "uploadfile", /* controller里,执行上传视频的action名称 */
                            fileFieldName = "upfile", /* 提交的文件表单名称 */
                            filePathFormat = CustomConfig.EditorFilePath, /* 上传保存路径,可以自定义保存路径和文件名格式 */
                            fileUrlPrefix = "", /* 文件访问路径前缀 */
                            fileMaxSize = CustomConfig.CanUploadFileSize * 1024 * 1024, /* 上传大小限制，单位B，默认50MB */
                            fileAllowFiles = CustomConfig.FileExts, /* 上传文件格式显示 */

                            /* 列出指定目录下的图片 */
                            imageManagerActionName = "listimage", /* 执行图片管理的action名称 */
                            imageManagerListPath = CustomConfig.EditorImgPath, /* 指定要列出图片的目录 */
                            imageManagerListSize = 20, /* 每次列出文件数量 */
                            imageManagerUrlPrefix = "", /* 图片访问路径前缀 */
                            imageManagerInsertAlign = "none", /* 插入的图片浮动方式 */
                            imageManagerAllowFiles = CustomConfig.ImgExts, /* 列出的文件类型 */

                            /* 列出指定目录下的文件 */
                            fileManagerActionName = "listfile", /* 执行文件管理的action名称 */
                            fileManagerListPath = CustomConfig.EditorFilePath, /* 指定要列出文件的目录 */
                            fileManagerUrlPrefix = "", /* 文件访问路径前缀 */
                            fileManagerListSize = 20, /* 每次列出文件数量 */
                            fileManagerAllowFiles = CustomConfig.FileExts /* 列出的文件类型 */
                        }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取结果状态字符串
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private string GetStateString(ResultState state)
        {
            switch (state)
            {
                case ResultState.Success:
                    return "SUCCESS";
                case ResultState.InvalidParam:
                    return "参数不正确";
                case ResultState.PathNotFound:
                    return "路径不存在";
                case ResultState.AuthorizError:
                    return "权限不足";
                case ResultState.IOError:
                    return "文件系统读取错误";
            }
            return "未知错误";
        }
	}
    public enum ResultState
    {
        Success,
        InvalidParam,
        AuthorizError,
        IOError,
        PathNotFound
    }
}