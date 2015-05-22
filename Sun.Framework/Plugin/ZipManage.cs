using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Web.Mvc;
using System.Threading.Tasks;
using System.IO;
using Sun.Core.Plugin;
using Newtonsoft.Json;
using Sun.Core.Ioc;

namespace Sun.Framework.Plugin
{
    public class ZipManage : IZipManage
    {
        static string zipsPath =Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,"Plugins","zips");
        static ZipManage _instance = new ZipManage();
        /// <summary>
        /// 启用单例模式
        /// </summary>
        public static ZipManage Instance
        {
            get
            {
                return _instance;
            }
        }
        private static ConcurrentDictionary<string, PluginZipInfo> zipInfoList = new ConcurrentDictionary<string, PluginZipInfo>();
        public void Initialize()
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "zips", "zipsinfo.json");
            var text = File.ReadAllText(filePath);
            var list = JsonConvert.DeserializeObject<List<PluginZipInfo>>(text);
            foreach (var zipInfo in list)
            {
                zipInfoList.TryAdd(zipInfo.ZipName, zipInfo);
            }
        }
        public IList<PluginZipInfo> GetPluginZips()
        {
            return zipInfoList.Values.ToList();
        }
        /// <summary>
        /// 安装插件包
        /// </summary>
        /// <param name="zipName"></param>
        public void InstallZip(string zipName)
        {
            IManage manage = Manage.Instance;
            PluginZipInfo zipInfo;
            if(zipInfoList.TryGetValue(zipName,out zipInfo))
            {
                PluginInfo pluginInfo=manage.GetPlugins().SingleOrDefault(p => p.Guid == zipInfo.Guid);
                if (pluginInfo != null)
                {
                    if (pluginInfo.Status == PluginStatus.NeedUnload)
                    {
                        throw new Exception("该插件刚被卸载,你需要重启应用后才能安装");
                    }
                    else
                    {
                        throw new Exception("该插件已安装,卸载插件并重启应用后才能再次安装");
                    }
                }
                pluginInfo = Loader.Instance.LoadFromZip(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "zips", zipInfo.ZipName));
                manage.InitPlugin(pluginInfo);
                manage.StopPlugin(pluginInfo.Guid); 
                WebIoc.Instance.Rebuild();
            }
        }
        /// <summary>
        /// 删除插件包
        /// </summary>
        /// <param name="zipName"></param>
        public void Delete(string zipName)
        {
            File.Delete(Path.Combine(zipsPath, zipName));
            PluginZipInfo zipInfo;
            if (zipInfoList.TryRemove(zipName, out zipInfo))
            {
                File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "zips", zipInfo.ZipName));
                SaveInfos();
            }
        }

        /// <summary>
        /// 保存信息
        /// </summary>
        void SaveInfos()
        {
            var text = JsonConvert.SerializeObject(GetPluginZips());
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins", "zips", "zipsinfo.json");
            File.WriteAllText(filePath, text);
        }

        /// <summary>
        /// 注册插件包
        /// </summary>
        /// <param name="zipName"></param>
        public void RegisterZip(string zipName)
        {
            ILoader loader = Loader.Instance;

            var pluginInfo = loader.LoadFromZip(Path.Combine(zipsPath, zipName), false);
            var zipInfo = new PluginZipInfo() { Guid = pluginInfo.Guid, Type = pluginInfo.Type, GroupName = pluginInfo.GroupName, NickName = pluginInfo.NickName, Version = pluginInfo.Version, Author = pluginInfo.Author, AssemblyNames = pluginInfo.AssemblyNames, Limit = pluginInfo.Limit, Intro = pluginInfo.Intro };
            zipInfo.ZipName = zipName;
            zipInfoList.AddOrUpdate(zipName, zipInfo,(n,p)=>p);
            SaveInfos();
        }
    }
}
