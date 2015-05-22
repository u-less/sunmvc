using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading;
using MongoDB.Driver.Core;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Sun.BaseOperate.Config;

namespace Plugin.MongoUpload
{
    public class SunMongoClient : MongoClient
    {
        public SunMongoClient():base()
        {}
        public SunMongoClient(MongoClientSettings settings)
            : base(settings)
        {}
        public SunMongoClient(MongoUrl url)
            : base(url)
        { }
        public SunMongoClient(string connectionString)
            : base(connectionString)
        { }
        /// <summary>
        /// 数据库组名
        /// </summary>
        public string DbGroupName
        {
            get;
            set;
        }
    }
    public class MongoDbServer
    {
        private static Dictionary<string, SunMongoClient> clientsDict = new Dictionary<string, SunMongoClient>();
        private static ReaderWriterLockSlim slimLock = new ReaderWriterLockSlim();
        static Random rd = new Random();
        static List<SunMongoClient> clients = new List<SunMongoClient>();
        /// <summary>
        /// 1:Mongodb服务地址需要更新,0:不需要更新
        /// </summary>
        public static bool HostNeedUpdate = true;
        /// <summary>
        /// 获取一个随机文件clent
        /// </summary>
        public static SunMongoClient MongoDbClient
        {
            get
            {
                if (HostNeedUpdate)
                {
                    slimLock.EnterWriteLock();
                    clients.Clear();
                    clientsDict.Clear();
                   var hosts =new List<SunMongoServer>() ;//CustomConfig.FilesServer;
                    foreach (var h in hosts)
                    {
                        if (h.State != 0)
                        {
                            var setting = new MongoClientSettings();
                            if (h.Hosts.Count > 1)
                            {
                                var servers = new List<MongoServerAddress>();
                                foreach (var o in h.Hosts)
                                {
                                    var pindex = o.LastIndexOf(':');
                                    var host = o.Substring(0, pindex);
                                    var port = int.Parse(o.Substring(pindex + 1));
                                    servers.Add(new MongoServerAddress(host, port));
                                }
                                setting.Servers = servers;
                                setting.ReplicaSetName = h.GroupName;
                                setting.ReadPreference = ReadPreference.Secondary;
                            }
                            else
                            {
                                var pindex = h.Hosts[0].LastIndexOf(':');
                                var host = h.Hosts[0].Substring(0, pindex);
                                var port = int.Parse(h.Hosts[0].Substring(pindex + 1));
                                setting.Server = new MongoServerAddress(host, port);
                            }
                            if (!string.IsNullOrEmpty(h.UserId))
                                setting.Credentials = new[] { MongoCredential.CreateMongoCRCredential("admin", h.UserId, h.PassWord) };
                            setting.MaxConnectionPoolSize = 1000;
                            setting.MinConnectionPoolSize = 50;
                            var client = new SunMongoClient(setting);
                            client.DbGroupName = h.GroupName;
                            if (h.State != 2)//满载禁止写入
                                clients.Add(client);
                            clientsDict.Add(h.GroupName, client);
                        }
                    }
                    slimLock.ExitWriteLock();
                    HostNeedUpdate = false;
                }
                return clients[rd.Next(clients.Count())];
            }
        }
        /// <summary>
        /// 通过group名获取MongoClent
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="client">clent</param>
        /// <returns>是否存在</returns>
        public static bool GetClentByGroupName(string groupName, out SunMongoClient client)
        {
            if (clientsDict.Count == 0)
            {var db=MongoDbClient; }
            return clientsDict.TryGetValue(groupName,out client);
        }
    }
    /// <summary>
    /// mongodb数据库对象
    /// </summary>
    public class SunMongoServer
    {
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }
        /// <summary>
        /// 组里服务器
        /// </summary>
        public List<string> Hosts
        {
            get;
            set;
        }
        /// <summary>
        /// 账号
        /// </summary>
        public string UserId
        {
            get;
            set;
        }
        /// <summary>
        /// 密码
        /// </summary>
        public string PassWord
        {
            get;
            set;
        }
        /// <summary>
        /// 状态0:不可用,1:正常,2:不可写(满载)
        /// </summary>
        public int State
        {
            get;
            set;
        }
    }
}
