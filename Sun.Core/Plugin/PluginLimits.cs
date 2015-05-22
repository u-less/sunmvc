using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
    public enum PluginLimits
    {
        /// <summary>
        /// 所有权限可用
        /// </summary>
        common=0,
        /// <summary>
        /// 禁止删除
        /// </summary>
        NoDelete=1,
        /// <summary>
        /// 禁止卸载
        /// </summary>
        NoUnload=2
    }
}
