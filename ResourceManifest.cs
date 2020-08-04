using OrchardCore.ResourceManagement;
using static Lombiq.DataTables.Constants.ResourceNames;

namespace Lombiq.DataTables
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(IResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // jQuery.DataTables-related resources.
            manifest
                .DefineScript(Uri_Js)
                .SetUrl("~/Lombiq.DataTables/Content/URI.js/URI.min.js", "~/Lombiq.DataTables/Content/URI.js/URI.js")
                .SetDependencies(JQuery);

            manifest
                .DefineScript(JQuery_DataTables)
                .SetDependencies(JQuery)
                .SetUrl("~/Lombiq.DataTables/vendors/datatables.net/jquery.dataTables.min.js", "~/Lombiq.DataTables/vendors/datatables.net/jquery.dataTables.js")
                .SetCdn("https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js", "https://cdn.datatables.net/1.10.20/js/jquery.dataTables.js")
                .SetCdnIntegrity("sha384-L74JDRkaoB7PWnReNepwX6+kSckc13TJXrka4EerY9jxQxSDl0dTguSLcA7dEfq8")
                .SetVersion("1.10.20");

            manifest
                .DefineScript(JQuery_DataTables_Bootstrap4)
                .SetDependencies(JQuery, JQuery_DataTables)
                .SetUrl("~/Lombiq.DataTables/vendors/datatables.net-bs4-js/dataTables.bootstrap4.min.js", "~/Lombiq.DataTables/vendors/datatables.net-bs4-js/dataTables.bootstrap4.js")
                .SetCdn("https://cdn.datatables.net/1.10.20/js/dataTables.bootstrap4.min.js", "https://cdn.datatables.net/1.10.20/js/dataTables.bootstrap4.js")
                .SetCdnIntegrity("sha384-dsXH1jw5mvdtskz6tkzogTCdKWJv4k12j2BOHq3okVzlZiIsQhQXSh0I86ggUPPf")
                .SetVersion("1.10.20");

            manifest
                .DefineStyle(JQuery_DataTables_Bootstrap4)
                .SetDependencies(Bootstrap)
                .SetUrl("~/Lombiq.DataTables/vendors/datatables.net-bs4-css/dataTables.bootstrap4.min.css", "~/Lombiq.DataTables/vendors/datatables.net-bs4-css/dataTables.bootstrap4.css")
                .SetCdn("https://cdn.datatables.net/1.10.20/css/dataTables.bootstrap4.min.css", "https://cdn.datatables.net/1.10.20/css/dataTables.bootstrap4.css")
                .SetCdnIntegrity("sha384-EkHEUZ6lErauT712zSr0DZ2uuCmi3DoQj6ecNdHQXpMpFNGAQ48WjfXCE5n20W+R")
                .SetVersion("1.10.20");

            manifest
                .DefineScript(JQuery_DataTables_AutoInit)
                .SetDependencies(JQuery_DataTables_Bootstrap4)
                .SetUrl("~/Lombiq.DataTables/finitive/jquery-datatables-autoinit/jquery-datatables-autoinit.js")
                .SetVersion("1.0");

            // Custom resources.
            manifest
                .DefineScript(Lombiq_DataTables)
                .SetUrl("~/Lombiq.DataTables/Scripts/lombiq-datatables.js")
                .SetDependencies(Uri_Js, JQuery_DataTables_Plugins_Processing);
            manifest
                .DefineScript(Lombiq_ContentPicker)
                .SetUrl("~/Lombiq.DataTables/Scripts/lombiq-contentpicker.js")
                .SetDependencies("jQueryColorBox");

            manifest
                .DefineStyle(Lombiq_ContentPicker)
                .SetDependencies("jQueryColorBox");
        }
    }
}
