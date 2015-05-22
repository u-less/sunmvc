using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Sun.Core.Ioc
{
    public class PluginIoc
    {
        static ContainerBuilder builder=new ContainerBuilder();
        static IContainer container;
        static List<RegisterFunc> RegisterFuncList = new List<RegisterFunc>();
        static object lockOjb = new object();
        public static void Register(Action<ContainerBuilder> func, string assemblyGuid = null)
        {
            RegisterFuncList.Add(new RegisterFunc() { AssemblyGuid = assemblyGuid, Func = func,IsRegister=true });
        }
        /// <summary>
        /// 构建容器
        /// </summary>
        public static void Build()
        {
            lock (lockOjb)
            {
                if (null == container)
                {
                    foreach (var rf in RegisterFuncList.Where(r => r.IsRegister))
                    {
                        rf.Func(builder);
                    }
                    container = builder.Build();
                }
            }
        }
        /// <summary>
        /// 重新构建WebIoc
        /// </summary>
        public static void Rebuild()
        {
            lock (lockOjb)
            {
                var rebuilder = new ContainerBuilder();
                foreach (var rf in RegisterFuncList.Where(r=>r.IsRegister))
                {
                    rf.Func(rebuilder);
                }
                builder = rebuilder;
                container = builder.Build();
            }
        }
        /// <summary>
        /// 获取container
        /// </summary>
        public static IContainer Container
        {
            get
            {
                return container;
            }
        }
    }
}
