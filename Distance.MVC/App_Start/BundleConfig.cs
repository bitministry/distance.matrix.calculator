using System.Web.Optimization;

namespace Distance.MVC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/bootstrap-multiselect.js",
                      "~/Scripts/bootstrapValidator.min.js",
                      "~/Scripts/jquery.tmpl.min.js",
                      "~/Scripts/linq.min.js",
                      "~/Scripts/respond.js", 
                      "~/Scripts/app/custom.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/themes/jquery-ui.css",
                      "~/Content/themes/base/progressbar.css",
                      "~/Content/bootstrap.css",
                      "~/Content/bootstrap-multiselect.css",
                      "~/Content/bootstrapValidator.css",
                      "~/Content/site.css"));
        }
    }
}
