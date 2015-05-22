using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using MongoDB.Driver.GridFS;
using MongoDB.Driver;
using MongoDB.Driver.Core;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using System.IO;
using System.Text.RegularExpressions;
using Sun.Framework.SunImg;
using Sun.BaseOperate.Config;
using Sun.Core.Upload;

namespace Plugin.MongoUpload
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
            var server = MongoDbServer.MongoDbClient;
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
                            imgCollection.InsertOneAsync(sunfile);
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
                    Result.Url += "/files/images/" + server.DbGroupName + "/" + collection + "/" + sunfile.Id;
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
        public UploadResult UseAttrUploadImg(HttpContextBase context, string collection, string pictitle = null, CutImageType t = CutImageType.Original, int width = 0, int height = 0)
        {
            var server = MongoDbServer.MongoDbClient;
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
                        case CutImageType.Thumbnail:
                            {
                                sunfile.Data = SunImage.GetImgThumbnail(file.InputStream, width, q, fileExtName).GetBuffer();
                            }; break;
                        case CutImageType.CutForCustom:
                            {
                                sunfile.Data = SunImage.CutForCustom(file.InputStream, width, height, q, fileExtName).GetBuffer();
                            }; break;
                        case CutImageType.CutForSquare:
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
                            imgCollection.InsertOneAsync(sunfile);
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
                    Result.Url += "/files/images/" + server.DbGroupName + "/" + collection + "/" + sunfile.Id;
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
            var imgCollection = client.GetDatabase(originalImgDatabase).GetCollection<SunFile>(collection);
            var filter = Builders<SunFile>.Filter.Gt(o => o.Time, DateTime.Now.AddMinutes(-20));
            var sort = Builders<SunFile>.Sort.Descending(f => f.Time);
            var task = imgCollection.Find(filter).Sort(sort).Skip(start - 1).Limit(size).ToListAsync();
            task.Wait();
            foreach (var o in task.Result)
            {
                yield return CustomConfig.FilesDomain + "/files/images/" + client.DbGroupName + "/" + collection + "/" + o.Id;
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
            var fileCollection = client.GetDatabase(fileDatabase).GetCollection<SunFile>(collection);
            var filter = Builders<SunFile>.Filter.Gt(o => o.Time, DateTime.Now.AddMinutes(-20));
            var sort = Builders<SunFile>.Sort.Descending(f => f.Time);
            var task = fileCollection.Find(filter).Sort(sort).Skip(start - 1).Limit(size).ToListAsync();
            task.Wait();
            foreach (var o in task.Result)
            {
                yield return CustomConfig.FilesDomain + "/files/SmallFile/" + client.DbGroupName + "/" +o.Id;
            }
        }
        /// <summary>
        /// 文件上传
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public UploadResult UploadFile(HttpContextBase context,string collection)
        {
            var server = MongoDbServer.MongoDbClient;
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
                        imgCollection.InsertOneAsync(sunfile);
                        Result.Url += "/files/SmallFile/" + server.DbGroupName + "/" + collection + "/" + fileId;
                        Result.State = UploadState.Success;
                    }
                    else
                    {
                        MongoGridFS gfs = new MongoGridFS(server.GetServer(), fileDatabase, new MongoGridFSSettings());
                        gfs.Upload(file.InputStream, fileId);
                        Result.Url += "/files/LargeFile/" + server.DbGroupName + "/" + fileId;
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
            SunMongoClient server;
            if (!MongoDbServer.GetClentByGroupName(groupName, out server))
                throw new Exception("数据库群组名不存在");
            var origDatabase = server.GetDatabase(MUpload.originalImgDatabase);//原图数据库
            var origCollection = origDatabase.GetCollection<SunFile>(collection);
            var filter = Builders<SunFile>.Filter.Eq(f=>f.Id,file.Id);
            origCollection.ReplaceOneAsync(filter,file);
            return true;
        }
        /// <summary>
        /// 获取图片数据
        /// </summary>
        /// <param name="groupName">数据库组名</param>
        /// <param name="collection">文件所在的集合</param>
        /// <param name="id">文件id</param>
        /// <returns></returns>
        public static async Task<SunFile> GetImg(string groupName = null, string collection = null, string id = null)
        {
            try
            {
                SunMongoClient server;
                if (!MongoDbServer.GetClentByGroupName(groupName, out server))
                    throw new Exception("数据库群组名不存在");
                var origDatabase = server.GetDatabase(MUpload.originalImgDatabase);//原图数据库
                var origCollection = origDatabase.GetCollection<SunFile>(collection);
                return await origCollection.Find<SunFile>(f=>f.Id==id).FirstOrDefaultAsync();
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
        public static async Task<SunFile> GetImg(string url)
        {
            var urlInfo = GetUrlInfo(url);
            return await GetImg(urlInfo.GroupName,urlInfo.Collection,urlInfo.FileId);
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
                SunMongoClient server;
                if (!MongoDbServer.GetClentByGroupName(dbGroupName, out server))
                    throw new Exception("数据库群组名不存在");
                var origDatabase = server.GetDatabase(MUpload.originalImgDatabase);//原图数据库
                var origCollection = origDatabase.GetCollection<SunFile>(collection);
                var editCollection = server.GetDatabase(MUpload.editImgDatabase).GetCollection<SunFile>(collection);//缩略图数据库
                origCollection.DeleteOneAsync<SunFile>(f=>f.Id==id);
                var filter = Builders<SunFile>.Filter.ElemMatch(f => f.Id, @"^\w+" + id + "$");
                editCollection.DeleteOneAsync(filter);
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
