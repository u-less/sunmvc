using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
    public enum PluginStatus
    {
        /// <summary>
        /// 插件错误
        /// </summary>
        Error = 0,
        /// <summary>
        /// 可用
        /// </summary>
        Usable = 1,
        /// <summary>
        /// 已停用
        /// </summary>
        Stop = 2,
        /// <summary>
        /// 插件需要卸载
        /// </summary>
        NeedUnload=3,
        /// <summary>
        /// 需要加载
        /// </summary>
        NeedLoad = 4
    }
}
