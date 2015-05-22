using System.Web;
using System.Web.Optimization;
using Sun.Framework.Plugin.MVC;

namespace Plugin.Admin
{
    public class BundleConfig
    {
        public static void RegisterBundles()
        {
            BundleManage.Add(bundles => bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Content/Scripts/jquery-1.8.3.min.js", "~/Content/Scripts/laytpl.js", "~/Content/Scripts/library.js"))
                        );
            BundleManage.Add(bundles => bundles.Add(new ScriptBundle("~/Content/Scripts/easyuiext/").Include(
                "~/Content/Scripts/easyui/locale/easyui-lang-zh_CN.js", "~/Content/Scripts/easyui/validateExt.js"))
                );
            BundleManage.Add(bundles => bundles.Add(new StyleBundle("~/Content/Scripts/easyui/themes/default/style").Include("~/Content/Scripts/easyui/themes/default/easyui.css"))
            );
        }
    }
}