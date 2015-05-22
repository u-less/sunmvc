
using System;
using Autofac;

namespace Sun.Core.Ioc
{
    internal class RegisterFunc
    {
        public string AssemblyGuid
        {
            get;
            set;
        }
        public Action<ContainerBuilder> Func
        {
            get;
            set;
        }
        public bool IsRegister
        {
            get;
            set;
        }
    }
}
