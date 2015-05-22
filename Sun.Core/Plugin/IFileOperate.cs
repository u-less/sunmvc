using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
    public interface IFileOperate
    {

        /// <summary>
        /// 根据文件路径获取插件信息
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        PluginInfo ParsePluginInfo(string filePath);
        /// <summary>
        /// 根据文件对象获取插件信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        PluginInfo ParsePluginInfo(FileInfo file);
        /// <summary>
        /// 保存插件信息
        /// </summary>
        /// <param name="plugin"></param>
        void SavePluginInfo(PluginInfo plugin);
        /// <summary>
        /// 删除插件
        /// </summary>
        /// <param name="plugin"></param>
        void DeletePlugin(PluginInfo plugin);
    }
}
