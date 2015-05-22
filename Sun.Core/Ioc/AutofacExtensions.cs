using System;
using System.Linq;
using System.Collections.Generic;
using Autofac;
using System.IO;
using System.Reflection;

namespace Sun.Core.Ioc
{
    public class DllCompaire : IEqualityComparer<FileInfo>
    {

        public bool Equals(FileInfo x, FileInfo y)
        {
            return x.Name == y.Name;
        }

        public int GetHashCode(FileInfo obj)
        {
           return obj.GetHashCode();
        }
    }
    public static class AutofacExtensions
    {
        /// <summary>
        /// 注册指定文件目录下面的所有文件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileFilter"></param>
        /// <param name="directoryPaths"></param>
        public static void RegisterTypeFromDirectory(this ContainerBuilder builder, Func<string, bool> fileFilter = null,params string[] directoryPaths)
        {
            foreach (var dpath in directoryPaths)
            {
                DirectoryInfo dInfo = new DirectoryInfo(dpath);
                var files = fileFilter != null ? dInfo.GetFiles("*.dll", SearchOption.AllDirectories).Where(f => fileFilter(f.Name)) : dInfo.GetFiles("*.dll", SearchOption.AllDirectories);
                foreach (var file in files.Distinct(new DllCompaire()))
                {
                    builder.RegisterTypeFromFile(file.FullName);
                }
            }
        }
        /// <summary>
        /// 注册指定文件路径的文件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="fileFullName"></param>
        public static void RegisterTypeFromFile(this ContainerBuilder builder, string fileFullName)
        {
            var assembly = Assembly.LoadFile(fileFullName);
            assembly = AppDomain.CurrentDomain.Load(assembly.GetName());
            var allClass = from types in assembly.GetExportedTypes()
                           where types.IsClass
                           select types;
            foreach (var c in allClass)
            {
                var exportAttrs = c.GetCustomAttributes(typeof(IocExportAttribute), false);
                if (exportAttrs.Length > 0)
                {
                    var exportAttr = exportAttrs[0] as IocExportAttribute;
                    if (null != exportAttr.ContractKey)
                    {
                        if(exportAttr.SingleInstance)
                            builder.RegisterType(c).Keyed(exportAttr.ContractKey, exportAttr.ContractType).SingleInstance();
                        else
                            builder.RegisterType(c).Keyed(exportAttr.ContractKey, exportAttr.ContractType);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(exportAttr.ContractName))
                        {
                            if (exportAttr.SingleInstance)
                                builder.RegisterType(c).Named(exportAttr.ContractName, exportAttr.ContractType).SingleInstance();
                            else
                                builder.RegisterType(c).Named(exportAttr.ContractName, exportAttr.ContractType);
                        }
                        else 
                        {
                            if (exportAttr.SingleInstance)
                                builder.RegisterType(c).As(exportAttr.ContractType).SingleInstance();
                            else
                                builder.RegisterType(c).As(exportAttr.ContractType);
                        }
                    }
                }
            }
        }
    }
}
