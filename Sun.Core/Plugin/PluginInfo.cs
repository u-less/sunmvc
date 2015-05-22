using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Sun.Core.Ioc;
using Autofac;

namespace Sun.Core.Plugin
{
    public class PluginInfo
    {
        /// <summary>
        /// 插件GUID(一般用程序集Guid)
        /// </summary>
        public string Guid
        {
            get; set;
        }
        /// <summary>
        /// 目录名称
        /// </summary>
        public string DirectoryName
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
        /// 插件位置(动态生成,无需在配置文件配置)
        /// </summary>
        public string Path
        {
            get;
            set;
        }
        /// <summary>
        /// 插件别名
        /// </summary>
        public string NickName
        {
            get;set;
        }
        /// <summary>
        /// 控制器
        /// </summary>
        public Dictionary<string,string> Areas
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
            get;set;
        }
        /// <summary>
        /// 插件入口
        /// </summary>
        public IPlugin Plugin
        {
            get
            {
                return PluginIoc.Container.ResolveNamed<IPlugin>(Guid);
            }
        }
        Assembly _PluginAssembly;
        /// <summary>
        /// 插件所在程序集
        /// </summary>
        public Assembly PluginAssembly
        {
            get {
                if (_PluginAssembly == null)
                    _PluginAssembly = Plugin.GetType().Assembly;
                return _PluginAssembly;
            }
            set {
                _PluginAssembly = value;
            }
        }
        List<Type> _Types;
        /// <summary>
        /// 插件包含的所有type
        /// </summary>
        public List<Type> Types
        {
            get
            {
                if (null==_Types)
                    _Types = Plugin.GetType().Assembly.GetExportedTypes().Where(t=>t.IsClass).ToList();
                return _Types;
            }
            set {
                _Types = value;
            }
        }
        /// <summary>
        /// 插件配置json字符串,使用的时候需要转换
        /// </summary>
        public string Config
        {
            get; set;
        }
        /// <summary>
        /// 主要程序集名称(设置错误可能导致程序异常)
        /// </summary>
        public List<string> AssemblyNames
        {
            get; set;
        }
        /// <summary>
        /// 插件显示顺序
        /// </summary>
        public int SortIndex
        {
            get;set;
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
        /// 状态：0无效,1可用,2已停用
        /// </summary>
        public PluginStatus Status
        {
            get;
            set;
        }
        /// <summary>
        /// 插件简介
        /// </summary>
        public string Intro
        {
            get; set;
        }
    }
}
