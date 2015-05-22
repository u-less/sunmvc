using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Optimization;

namespace Sun.Framework.Plugin.MVC
{
   public class BundleManage
    {
        static event Action<BundleCollection> BundleHandlers;
        public static void Add(Action<BundleCollection> bundleHandler)
        {
            BundleHandlers += bundleHandler;
        }
        public static void RegisterAllBundles(BundleCollection bundles)
        {
            if (bundles != null && BundleHandlers != null)
                BundleHandlers(bundles);
            BundleHandlers = null;//释放事件
        }
    }
}
