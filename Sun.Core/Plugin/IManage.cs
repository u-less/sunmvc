using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
	public interface IManage
	{
		/// <summary>
		/// 初始化。
		/// </summary>
		void Initialize();
		/// <summary>
		/// 初始化插件。
		/// </summary>
		/// <param name="pluginInfo">插件描述</param>
		void InitPlugin(PluginInfo pluginInfo);
		/// <summary>
		/// 卸载指定的插件
		/// </summary>
		/// <param name="guid"></param>
		void UnloadPlugin(string guid);
		/// <summary>
		/// 获取所有插件信息
		/// </summary>
		/// <returns></returns>
		IEnumerable<PluginInfo> GetPlugins();
		/// <summary>
		/// 根据插件名获取插件信息
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		PluginInfo GetPluginByName(string name);
        /// <summary>
        /// 根据area获取插件信息(mvc插件使用)
        /// </summary>
        /// <param name="areaName"></param>
        /// <returns></returns>
        PluginInfo GetPluginByAreaName(string areaName);
		/// <summary>
		/// 通过插件guid获取插件信息
		/// </summary>
		/// <param name="guid"></param>
		/// <returns></returns>
		PluginInfo GetPluginById(string guid);
		/// <summary>
		/// 启动插件
		/// </summary>
		void StartPlugin(string guid);
		/// <summary>
		/// 停止插件
		/// </summary>
		/// <param name="guid"></param>
		void StopPlugin(string guid);
	}
}
