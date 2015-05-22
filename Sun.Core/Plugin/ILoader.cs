using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
    public interface ILoader
    {
        /// <summary>
        /// 插件所在路径
        /// </summary>
        string PluginBasePath { get; }
        /// <summary>
        /// 插件临时文件路径
        /// </summary>
        string TempBasePath { get; }
        /// <summary>
        /// 插件自述信息文件名
        /// </summary>
        string PluginInfoFileName { get; }
        /// <summary>
        /// 从插件目录加载插件。
        /// </summary>
        /// <returns></returns>
        IEnumerable<PluginInfo> Load();
        /// <summary>
        /// 从zip文件中加载插件
        /// </summary>
        /// <param name="zipPath"></param>
        /// <param name="load">是否加载(设置为false则只读取插件信息而不加载插件到插件容器)</param>
        /// <returns></returns>
        PluginInfo LoadFromZip(string zipPath,bool load = true);
    }
}
