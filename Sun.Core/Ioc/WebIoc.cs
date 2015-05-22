//author:无名书生 2013/3/29
//intro:web部分IOC统一入口点
using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace Sun.Core.Ioc
{
    public class WebIoc
    {
        static ContainerBuilder builder= new ContainerBuilder();
        static IContainer container;
        static List<RegisterFunc> RegisterFuncList = new List<RegisterFunc>();
        static object lockOjb = new object();
        static WebIoc ioc = new WebIoc();
        public static WebIoc Instance
        {
           get {
                return ioc;
            }
        }
        public static void Register(Action<ContainerBuilder> func, string assemblyGuid = null, bool isRegister = true)
        {
            RegisterFuncList.Add(new RegisterFunc() { AssemblyGuid= assemblyGuid,Func=func,IsRegister= isRegister});
        }
        public  void Build()
        {
            foreach(var rf in RegisterFuncList.Where(r => r.IsRegister))
            {
                rf.Func(builder);
            }
            container = builder.Build();
            OnBuilded(container);
        }
        /// <summary>
        /// 重新构建WebIoc
        /// </summary>
        public void Rebuild()
        {
           lock(lockOjb)
            {
                var rebuilder = new ContainerBuilder();
                foreach (var rf in RegisterFuncList.Where(r=>r.IsRegister))
                {
                    rf.Func(rebuilder);
                }
                builder = rebuilder;
                container = builder.Build();
                OnBuilded(container);
            }
        }
        /// <summary>
        /// 停止相关程序集的注入
        /// </summary>
        /// <param name="assemblyGuid"></param>
        public void StopAssembly(string assemblyGuid)
        {
            if (!string.IsNullOrEmpty(assemblyGuid))
            {
                var needRebuild = RegisterFuncList.Where(f => f.AssemblyGuid == assemblyGuid && f.IsRegister == true).ToList();
                if (needRebuild.Count() > 0)
                {
                    foreach (var r in needRebuild)
                    {
                        r.IsRegister = false;
                    }
                    Rebuild();
                }
            }
        }
        /// <summary>
        /// 启动相关程序集的注入
        /// </summary>
        /// <param name="assemblyGuid"></param>
        public void StartAssembly(string assemblyGuid)
        {
            if (!string.IsNullOrEmpty(assemblyGuid))
            {
               var needRebuild=RegisterFuncList.Where(f => f.AssemblyGuid == assemblyGuid&&f.IsRegister==false).ToList();
                if (needRebuild.Count() > 0)
                {
                    foreach (var r in needRebuild)
                    {
                        r.IsRegister = true;
                    }
                    Rebuild();
                }
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
        /// <summary>
        /// 容器构建事件(build和rebuild时触发)
        /// </summary>
        public event Action<IContainer> OnBuilded;
    }
}
