using System.Web;
using System.Web.Optimization;

namespace MUSICNOW.Web
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                            "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                            "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                            "~/Scripts/modernizr-*"));

            // Thay vì new ScriptBundle(...)
            var bootstrapBundle = new Bundle("~/bundles/bootstrap"); // Dùng new Bundle()
            bootstrapBundle.Include("~/Scripts/bootstrap.bundle.min.js");
            bundles.Add(bootstrapBundle);

            // Sửa StyleBundle để bao gồm site-custom.css
            bundles.Add(new StyleBundle("~/Content/css").Include(
                            "~/Content/bootstrap.css",
                            "~/Content/site.css",
                            "~/Content/site-custom.css")); // <-- DÒNG BỔ SUNG

            // Bật tối ưu hóa cho Production (Tùy chọn)
            // BundleTable.EnableOptimizations = true;

        }
    }
}