using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
    public class PluginZipInfo
    {
        /// <summary>
        /// 程序集的GUID
        /// </summary>
        public string Guid
        {
            get;
            set;
        }
        /// <summary>
        /// 插件类名(模块插件、功能插件等)
        /// </summary>
        public PluginType Type
        {
            get;
            set;
        }
        /// <summary>
        /// 插件组名
        /// </summary>
        public string GroupName
        {
            get;
            set;
        }
        /// <summary>
        /// 插件别名
        /// </summary>
        public string NickName
        {
            get;
            set;
        }

        /// <summary>
        /// 插件版本
        /// </summary>
        public string Version
        {
            get;
            set;
        }
        /// <summary>
        /// 插件作者
        /// </summary>
        public string Author
        {
            get;
            set;
        }
        /// <summary>
        /// 主要程序集名称(设置错误可能导致程序异常)
        /// </summary>
        public List<string> AssemblyNames
        {
            get;
            set;
        }
        /// <summary>
        /// 权限信息
        /// </summary>
        public PluginLimits Limit
        {
            get;
            set;
        }
        /// <summary>
        /// 插件简介
        /// </summary>
        public string Intro
        {
            get;
            set;
        }
        /// <summary>
        /// 安装包名称
        /// </summary>
        public string ZipName
        {
            get;
            set;
        }
    }
}
