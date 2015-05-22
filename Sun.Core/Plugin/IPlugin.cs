using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// 名称。
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取插件信息
        /// </summary>
        PluginInfo Info { get; }
        /// <summary>
        /// 初始化(每次启动执行)
        /// </summary>
        void Initialize(PluginInfo info);
        /// <summary>
        /// 启用插件
        /// </summary>
        void Start();
        /// <summary>
        /// 停用插件
        /// </summary>
        void Stop();
    }
}
