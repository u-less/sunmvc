using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sun.Core.Plugin;
using Sun.Core.Ioc;
using Newtonsoft.Json;

namespace Sun.Framework.Plugin
{
    public class FileOperate:IFileOperate
   {
        static FileOperate _instance=new FileOperate();
        public static FileOperate Instance
        {
            get
            {
                return _instance;
            }
        }
        /// <summary>
        /// 解析插件信息
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public PluginInfo ParsePluginInfo(string filePath)
        {
            if (!File.Exists(filePath))
                throw new Exception("插件自述文件不存在");
            var text = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<PluginInfo>(text);
        }
        /// <summary>
        /// 保存插件信息
        /// </summary>
        /// <param name="plugin">插件对象</param>
        /// <param name="filePath">保存地址</param>
        public void SavePluginInfo(PluginInfo plugin)
        {
            var path = Path.Combine(plugin.Path, Loader.Instance.PluginInfoFileName);
            if (!File.Exists(path))
                File.Create(path);
            if (plugin.Type == PluginType.Module)
                File.WriteAllText(path, JsonConvert.SerializeObject(new { Guid = plugin.Guid, Type = plugin.Type, Areas = plugin.Areas, GroupName = plugin.GroupName, Name = plugin.Plugin.Name, NickName = plugin.NickName, Version = plugin.Version, Author = plugin.Author, AssemblyNames = plugin.AssemblyNames, Status = plugin.Status, Config = plugin.Config, Intro = plugin.Intro }));
            else
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(new { Guid = plugin.Guid, Type = plugin.Type, GroupName = plugin.GroupName, Name = plugin.Plugin.Name, NickName = plugin.NickName, Version = plugin.Version, Author = plugin.Author, AssemblyNames = plugin.AssemblyNames, Status = plugin.Status, Config = plugin.Config, Intro = plugin.Intro }));
            }
        }
        /// <summary>
        /// 从文件对象中读取插件信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public PluginInfo ParsePluginInfo(FileInfo file)
        {
            if (!file.Exists)
                throw new Exception("插件自述文件不存在");
            StringBuilder result = new StringBuilder();
            using (var stream = file.OpenText())
            { 
                string str;
                while((str=stream.ReadLine())!=null)
                {
                    result.Append(str);
                }
            }
            return JsonConvert.DeserializeObject<PluginInfo>(result.ToString());
        }
        public void DeletePlugin(PluginInfo plugin)
        {
            File.Delete(plugin.Path);
        }
   }
}
