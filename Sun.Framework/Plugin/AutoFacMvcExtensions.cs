using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using Autofac;
using Autofac.Integration.Mvc;
using Sun.Core.Ioc;

namespace Sun.Framework.Plugin
{
    public static class AutoFacMvcExtensions
    {
        /// <summary>
        /// 注册文件夹里面所有控制器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileFilter"></param>
        /// <param name="directoryPaths"></param>
        public static void RegisterControllersFromDirectory(this ContainerBuilder builder, Func<string, bool> fileFilter = null, params string[] directoryPaths)
        {
            List<Assembly> assemblys = new List<Assembly>();
            foreach (var dpath in directoryPaths)
            {
                DirectoryInfo dInfo = new DirectoryInfo(dpath);
                var files = fileFilter != null ? dInfo.GetFiles("*.dll", SearchOption.AllDirectories).Where(f => fileFilter(f.Name)) : dInfo.GetFiles("*.dll", SearchOption.AllDirectories);
                foreach (var file in files.Distinct(new DllCompaire()))
                {
                    var assembly = Assembly.LoadFile(file.FullName);
                    assembly = AppDomain.CurrentDomain.Load(assembly.GetName());
                    assemblys.Add(assembly);
                }
            }
            builder.RegisterControllers(assemblys.ToArray());
        }
        /// <summary>
        /// 注册文件里面的控制器
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileFullName"></param>
        public static void RegisterControllersFromFile(this ContainerBuilder builder, string fileFullName)
        {
            var assembly = Assembly.LoadFile(fileFullName);
            assembly = AppDomain.CurrentDomain.Load(assembly.GetName());
            builder.RegisterControllers(assembly);
        }
    }
}
