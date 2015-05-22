using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using System.IO;
using Fang.BLL.Config;
using Fang.Model.Entity;
using Fang.BLL.Login;
using Fang.Library.SunFile;
using System.Text.RegularExpressions;
using Fang.Library.SunImg;

namespace Fang.BLL.MongoDb
{
    public class MUUrlInfo
    {
        /// <summary>
        /// 组名
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }
        /// <summary>
        /// 集合
        /// </summary>
        public string Collection
        {
            get;
            set;
        }
        /// <summary>
        /// 文件编号
        /// </summary>
        public string FileId
        {
            get;
            set;
        }
        /// <summary>
        /// 后缀
        /// </summary>
        public string Extension
        {
            get;
            set;
        }
    }
    public class MUpload
    {
        /// <summary>
        /// 分解url正则表达式
        /// </summary>
        private static Regex urlRegex = new Regex(@"^(?:.+)/(?<gname>\w+)/(?<cl>\w+)/(?<id>\w+\.(?<ext>\w+))$", RegexOptions.Compiled);
        /// <summary>
        /// 解析url包含的元数据
        /// </summary>
        /// <returns></returns>
        public static MUUrlInfo GetUrlInfo(string url)
        {
            var result = urlRegex.Match(url);
            return new MUUrlInfo
            {
                GroupName=result.Groups["gname"].Value,
                Collection=result.Groups["cl"].Value,
                FileId=result.Groups["id"].Value,
                Extension = result.Groups["ext"].Value
            };
        }

        /// <summary>
        /// 原图数据库
        /// </summary>
        public const string originalImgDatabase = "Images";
        /// <summary>
        ///缩略图数据库
        ///</summary>
        public const string editImgDatabase = "ImageThumbs";
        /// <summary>
        /// 文件数据库
        /// </summary>
        public const string fileDatabase = "files";
        public static List<string> filesDomains = new List<string>();
        public static bool filesDomainsNeedUpdate=true;
        private object lockObj = new object();
        private static Random rd = new Random();
        public string FileDomain
        {
            get
            {
                if (filesDomainsNeedUpdate)
                {
                    lock (lockObj)
                    {
                        filesDomains.Clear();
                        string[] domains = CustomConfig.FilesDomain.Split(',');
                        foreach (var d in domains)
                        {
                            filesDomains.Add(d);
                        }
                        filesDomainsNeedUpdate = false;
                    }
                }
                return filesDomains[rd.Next(filesDomains.Count())];
            }
        }
        /// <summary>
        /// mongodb图片上传
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection">集合</param>
        /// <param name="pictitle">图片标题</param>
        /// <param name="base64">是否是以base64上传的</param>
        /// <param name="inputName">图片表单域名称</param>
        /// <returns>上传结果</returns>
        public UploadResult UploadImg(HttpContextBase context, string collection, string pictitle = null, bool base64 = false, string inputName = "upfile")
        {
            var client = MongoDbServer.MongoDbClient;
            var server = client.GetServer();
            UploadResult Result = new UploadResult() { State = UploadState.Unknown };
            Result.Url = FileDomain;
            byte[] fileData = null;
            string fileExtName = null;
            double size = 0;//M
            if (base64)
            {
                fileExtName = ".png";
                if (string.IsNullOrEmpty(pictitle))
                    pictitle = inputName + fileExtName;
                fileData = Convert.FromBase64String(context.Request[inputName]);
                size = fileData.Length * 1.0 / 1024 / 1024;
            }
            else
            {
                HttpPostedFileBase file = context.Request.Files[0];
                size = file.ContentLength * 1.0 / 1024 / 1024;
                fileExtName = System.IO.Path.GetExtension(file.FileName).ToLower();
                if (string.IsNullOrEmpty(pictitle))
                    pictitle = file.FileName;
                fileData = new byte[file.ContentLength];
                file.InputStream.Read(fileData, 0, fileData.Length);

            }
            if (CustomConfig.ImgExts.Contains(fileExtName))
            {
                var tick = DateTime.Now.Ticks;
                Random rd = new Random(unchecked((int)tick));
                var imgId = tick.ToString();
                if (size < CustomConfig.CanUploadImgSize)
                {

                    SunFile sunfile = new SunFile();
                    sunfile.Time = DateTime.Now;
                    if (size > 0.5)
                        sunfile.Data = SunImage.ImgReduce(new MemoryStream(fileData), CustomConfig.ImgQuality, fileExtName).GetBuffer();
                    else
                        sunfile.Data = fileData;
                    var imgCollection = server.GetDatabase(originalImgDatabase).GetCollection<SunFile>(collection);
                    var rdId = rd.Next(99999);
                    int count = 0;
                    while (true)
                    {
                        try
                        {
                            sunfile.Id = imgId + rdId + fileExtName;
                            imgCollection.Insert(sunfile);
                            break;
                        }
                        catch
                        {
                            if (count > 3)
                            {
                                Result.State = UploadState.FileAccessError;
                                return Result;
                            }
                            rdId = rd.Next(99999);
                        }
                        count++;
                    }
                    Result.Url += "/files/images/" + client.DbGroupName + "/" + collection + "/" + sunfile.Id;
                    Result.OriginFileName = pictitle;
                    Result.State = UploadState.Success;
                    Result.FileId = sunfile.Id;
                }
                else
                {
                    Result.State = UploadState.SizeLimitExceed;
                }
            }
            else { Result.State = UploadState.TypeNotAllow; }
            return Result;
        }
        /// <summary>
        /// 带参数图片上传
        /// </summary>
        /// <param name="context"></param>
        /// <param name="collection">集合</param>
        /// <param name="pictitle">图片标题</param>
        /// <param name="t">图片处理方式</param>
        /// <param name="width">宽</param>
        /// <param name="height">高</param>
        /// <returns></returns>
        public UploadResult UseAttrUploadImg(HttpContextBase context, string collection, string pictitle = null, ResizeImgType t = ResizeImgType.Original, int width = 0, int height = 0)
        {
            var client = MongoDbServer.MongoDbClient;
            var server = client.GetServer();
            UploadResult Result = new UploadResult() { State = UploadState.Unknown };
            Result.Url = FileDomain;
            string fileExtName = null;
            double size = 0;//M
            HttpPostedFileBase file = context.Request.Files[0];
            size = file.ContentLength * 1.0 / 1024 / 1024;
            fileExtName = System.IO.Path.GetExtension(file.FileName).ToLower();
            if (string.IsNullOrEmpty(pictitle))
                pictitle = file.FileName;
            if (CustomConfig.ImgExts.Contains(fileExtName))
            {
                var tick = DateTime.Now.Ticks;
                Random rd = new Random(unchecked((int)tick));
                var imgId = tick.ToString();
                if (size < CustomConfig.CanUploadImgSize)
                {
                    SunFile sunfile = new SunFile();
                    sunfile.Time = DateTime.Now;
                    int q = size > 0.5 ? CustomConfig.ImgQuality : 100;
                    switch (t)
                    {
                        case ResizeImgType.Thumbnail:
                            {
                                sunfile.Data = SunImage.GetImgThumbnail(file.InputStream, width, q, fileExtName).GetBuffer();
                            }; break;
                        case ResizeImgType.CutForCustom:
                            {
                                sunfile.Data = SunImage.CutForCustom(file.InputStream, width, height, q, fileExtName).GetBuffer();
                            }; break;
                        case ResizeImgType.CutForSquare:
                            {
                                sunfile.Data = SunImage.CutForSquare(file.InputStream, width, q, fileExtName).GetBuffer();
                            }; break;
                        default:
                            {
                                if (q!=100)
                                    sunfile.Data = SunImage.ImgReduce(file.InputStream,q, fileExtName).GetBuffer();
                                else
                                {
                                    sunfile.Data = new byte[file.ContentLength];
                                    file.InputStream.Read(sunfile.Data, 0, sunfile.Data.Length);
                                }
                            }; break;
                    }
                    var imgCollection = server.GetDatabase(originalImgDatabase).GetCollection<SunFile>(collection);
                    var rdId = rd.Next(99999);
                    int count = 0;
                    while (true)
                    {
                        try
                        {
                            sunfile.Id = imgId + rdId + fileExtName;
                            imgCollection.Insert(sunfile);
                            break;
                        }
                        catch
                        {
                            if (count > 3)
                            {
                                Result.State = UploadState.FileAccessError;
                                return Result;
                            }
                            rdId = rd.Next(99999);
                        }
                        count++;
                    }
                    Result.Url += "/files/images/" + client.DbGroupName + "/" + collection + "/" + sunfile.Id;
                    Result.OriginFileName = pictitle;
                    Result.State = UploadState.Success;
                    Result.FileId = sunfile.Id;
                }
                else
                {
                    Result.State = UploadState.SizeLimitExceed;
                }
            }
            else { Result.State = UploadState.TypeNotAllow; }
            return Result;
        }
        /// <summary>
        /// 获取图片列表
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public IEnumerable<string> GetImgs(string collection, int start, int size)
        {
            var client = MongoDbServer.MongoDbClient;
            var imgCollection = client.GetServer().GetDatabase(originalImgDatabase).GetCollection(collection);
            var data = imgCollection.Find(Query<SunFile>.GT(o => o.Time, DateTime.Now.AddMinutes(-20))).SetSortOrder(new SortByDocument("Time",-1)).SetSkip(start-1).SetLimit(size);
            foreach (var o in data)
            {
                yield return CustomConfig.FilesDomain + "/files/images/" + client.DbGroupName + "/" + collection + "/" + o["_id"].AsString;
            }
        }
        /// <summary>
        /// 获取文件列表
        /// </summary>
        /// <param name="start">页码</param>
        /// <param name="size">每页文件数量</param>
        /// <returns></returns>
        public IEnumerable<string> GetFiles(string collection, int start, int size)
        {
            var client = MongoDbServer.MongoDbClient;
            var fileCollection = client.GetServer().GetDatabase(fileDatabase).GetCollection(collection);
            var data = fileCollection.Find(Query<SunFile>.GT(o => o.Time, DateTime.Now.AddMinutes(-20))).SetSortOrder(new SortByDocument("Time", -1)).SetSkip(start - 1).SetLimit(size);
            foreach (var o in data)
            {
                yield return CustomConfig.FilesDomain + "/files/SmallFile/" + client.DbGroupName + "/" + o["_id"].AsString;
            }
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public UploadResult UploadFile(HttpContextBase context,string collection)
        {
            var client = MongoDbServer.MongoDbClient;
            var server = client.GetServer();
            HttpPostedFileBase file = context.Request.Files[0];
            UploadResult Result = new UploadResult() { State = UploadState.Unknown };
            var Size = file.ContentLength * 1.0 / 1024 / 1024;
            var extName = System.IO.Path.GetExtension(file.FileName).ToLower();
            Result.Url = FileDomain;
            Result.OriginFileName=file.FileName;
            if (CustomConfig.FileExts.Contains(extName))
            {
                Random rd = new Random();
                var fileId = DateTime.Now.Ticks + rd.Next(9999) + extName;
                if (Size < CustomConfig.CanUploadFileSize)
                {
                    if (Size <= 6)
                    {
                        SunFile sunfile = new SunFile();
                        byte[] data = new byte[file.ContentLength];
                        file.InputStream.Read(data, 0, data.Length);
                        sunfile.Id = fileId;
                        sunfile.Data = data;
                        sunfile.Time = DateTime.Now;
                        var imgCollection = server.GetDatabase(fileDatabase).GetCollection<SunFile>(collection);
                        imgCollection.Insert(sunfile);

                        Result.Url += "/files/SmallFile/" + client.DbGroupName + "/" + collection +"/"+ fileId;
                        Result.State = UploadState.Success;
                    }
                    else
                    {
                        MongoGridFS gfs = new MongoGridFS(server, fileDatabase, new MongoGridFSSettings());
                        gfs.Upload(file.InputStream, fileId);
                        Result.Url += "/files/LargeFile/" + client.DbGroupName + "/" + fileId;
                        Result.State = UploadState.Success;
                    }
                    Result.FileId = fileId;
                }
                else
                {
                    Result.State = UploadState.SizeLimitExceed;
                }
            }
            else { Result.State = UploadState.TypeNotAllow; }
            return Result;
        }
        /// <summary>
        /// 修改图片数据
        /// </summary>
        /// <param name="groupName">数据库组</param>
        /// <param name="collection">集合</param>
        /// <param name="file">文件对象</param>
        /// <returns></returns>
        public bool UpdateImg(string groupName, string collection,SunFile file)
        {
            SunMongoClient clent;
            if (!MongoDbServer.GetClentByGroupName(groupName, out clent))
                throw new Exception("数据库群组名不存在");
            var server = clent.GetServer();
            var origDatabase = server.GetDatabase(MUpload.originalImgDatabase);//原图数据库
            var origCollection = origDatabase.GetCollection<SunFile>(collection);
            return origCollection.Save<SunFile>(file).Ok;
        }
        /// <summary>
        /// 获取图片数据
        /// </summary>
        /// <param name="groupName">数据库组名</param>
        /// <param name="collection">文件所在的集合</param>
        /// <param name="id">文件id</param>
        /// <returns></returns>
        public static SunFile GetImg(string groupName = null, string collection = null, string id = null)
        {
            try
            {
                SunMongoClient clent;
                if (!MongoDbServer.GetClentByGroupName(groupName, out clent))
                    throw new Exception("数据库群组名不存在");
                var server = clent.GetServer();
                var origDatabase = server.GetDatabase(MUpload.originalImgDatabase);//原图数据库
                var origCollection = origDatabase.GetCollection<SunFile>(collection);
                return origCollection.FindOne(Query<SunFile>.EQ(f => f.Id, id));
            }
            catch
            {
                throw new Exception("文件不存在");
            }
        }
        /// <summary>
        /// 获取图片数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static SunFile GetImg(string url)
        {
            var urlInfo = GetUrlInfo(url);
            return GetImg(urlInfo.GroupName,urlInfo.Collection,urlInfo.FileId);
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="dbGroupName">文件数据库组</param>
        /// <param name="collection">文件集合</param>
        /// <param name="id">文件ID</param>
        /// <returns></returns>
        public static bool DeleteFile(string dbGroupName,string collection,string id)
        {
            try
            {
                SunMongoClient clent;
                if (!MongoDbServer.GetClentByGroupName(dbGroupName, out clent))
                    throw new Exception("数据库群组名不存在");
                var server = clent.GetServer();
                var origDatabase = server.GetDatabase(MUpload.originalImgDatabase);//原图数据库
                var origCollection = origDatabase.GetCollection<SunFile>(collection);
                var editCollection = server.GetDatabase(MUpload.editImgDatabase).GetCollection<SunFile>(collection);//缩略图数据库
                origCollection.Remove(Query<SunFile>.EQ(f => f.Id, id));
                editCollection.Remove(Query<SunFile>.Matches(f => f.Id, @"^\w+" + id + "$"));
                return true;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="url">文件地址</param>
        /// <returns></returns>
        public static bool DeleteFile(string url)
        {
            var urlInfo = GetUrlInfo(url);
            return DeleteFile(urlInfo.GroupName, urlInfo.Collection, urlInfo.FileId);
        }
    }
}
