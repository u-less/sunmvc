using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sun.Core.Plugin
{
    public interface IZipManage
    {
        IList<PluginZipInfo> GetPluginZips();
        /// <summary>
        /// 初始化。
        /// </summary>
        void Initialize();
        /// <summary>
        /// 安装插件包 
        /// </summary>
        /// <param name="zipName"></param>
        void InstallZip(string zipName);
        /// <summary>
        /// 注册插件包
        /// </summary>
        /// <param name="zipName"></param>
        void RegisterZip(string zipName);
        /// <summary>
        /// 删除插件包
        /// </summary>
        /// <param name="zipName"></param>
        void Delete(string zipName);
    }
}
