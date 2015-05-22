using System;
using System.Collections.Concurrent;
using Sun.Core.Caching;
using StackExchange.Redis;
using System.IO;

namespace Plugin.RedisCache
{
    public class RedisCache : ICache
    {
        ConfigInfo conf;
        static ConcurrentDictionary<string, ConnectionMultiplexer> multiplexers = new ConcurrentDictionary<string, ConnectionMultiplexer>();
        public RedisCache(ConfigInfo config)
        {
            conf = config;
            CacheExpireSpan = new TimeSpan(0,config.CacheExpireMinutes,0);
        }

        /// <summary>
        /// redis连接端
        /// </summary>
        public ConnectionMultiplexer multiplexer
        {
            get
            {
                return GetMultiplexer(conf); ;
            }
        }
        public IDatabase DataBase
        {
            get
            {
                return multiplexer.GetDatabase(conf.DataBase);
            }
        }
        public TimeSpan CacheExpireSpan
        {
            get;
            set;
        }
        /// <summary>
        /// 获取client
        /// </summary>
        /// <returns></returns>
        private static ConnectionMultiplexer GetMultiplexer(ConfigInfo config)
        {
            ConnectionMultiplexer multiplexer = null;
            if (!multiplexers.TryGetValue(config.Hosts, out multiplexer))
            {
                var redisconf = ConfigurationOptions.Parse(config.Hosts);
                redisconf.AllowAdmin = true;
                redisconf.ConnectRetry = 2;
                multiplexer = ConnectionMultiplexer.Connect(redisconf);
                multiplexers.TryAdd(config.Hosts, multiplexer);
            }
            return multiplexer;
        }
        public void Delete(string key)
        {
            DataBase.KeyDelete(key);
        }

        public void Expire(string key, TimeSpan expire)
        {
            DataBase.KeyPersist(key);
            DataBase.KeyExpire(key, expire);
        }

        public bool Get<T>(string key, out T entity) where T : class
        {
            try
            {
                var value = DataBase.StringGet(key);
                if (!value.IsNullOrEmpty)
                {
                    entity = ProtoBuf.Serializer.Deserialize<T>(new MemoryStream(value));
                    return true;
                }
                else
                {
                    entity = null;
                    return false;
                }
            }
            catch
            {
                entity = null;
                return false;
            }
        }

        public void Set<T>(string key, T Entity, TimeSpan? expiry = default(TimeSpan?))
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                ProtoBuf.Serializer.Serialize<T>(ms, Entity);
                byte[] data = new byte[ms.Position];
                ms.Position = 0;
                ms.Read(data, 0, data.Length);
                ms.Dispose();
                DataBase.StringSet(key, data, expiry ?? CacheExpireSpan);
            }
            catch { }
        }

        public bool StringGet(string key, out string value)
        {
            try
            {
                var v = DataBase.StringGet(key);
                if (!v.IsNullOrEmpty)
                {
                    value = v;
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
            catch
            {
                value = null;
                return false;
            }
           
        }

        public void StringSet(string key, string value, TimeSpan? expiry = default(TimeSpan?))
        {
            DataBase.StringSet(key, value, expiry ?? CacheExpireSpan);
        }
    }
}
