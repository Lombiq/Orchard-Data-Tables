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
                .SetUrl("~/Lombiq.DataTables/vendors/urijs/URI.min.js", "~/Lombiq.DataTables/vendors/urijs/URI.js")
                .SetDependencies(JQuery);

            manifest
                .DefineScript(JQuery_DataTables)
                .SetDependencies(JQuery)
                .SetUrl("~/Lombiq.DataTables/vendors/datatables.net/jquery.dataTables.min.js", "~/Lombiq.DataTables/vendors/datatables.net/jquery.dataTables.js")
                //.SetCdn("https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js", "https://cdn.datatables.net/1.10.20/js/jquery.dataTables.js")
                //.SetCdnIntegrity("sha384-L74JDRkaoB7PWnReNepwX6+kSckc13TJXrka4EerY9jxQxSDl0dTguSLcA7dEfq8","sha384-FeGVmTD/nb8R8suJjHKxL3iAigW2uFc536mNbyfM60EY3KH6wit0Jmgx0/QO2reU")
                .SetVersion("1.10.20");

            manifest
                .DefineScript(JQuery_DataTables_Bootstrap4)
                .SetDependencies(JQuery, JQuery_DataTables)
                .SetUrl("~/Lombiq.DataTables/vendors/datatables.net-bs4-js/dataTables.bootstrap4.min.js", "~/Lombiq.DataTables/vendors/datatables.net-bs4-js/dataTables.bootstrap4.js")
                //.SetCdn("https://cdn.datatables.net/1.10.20/js/dataTables.bootstrap4.min.js", "https://cdn.datatables.net/1.10.20/js/dataTables.bootstrap4.js")
                //.SetCdnIntegrity("sha384-dsXH1jw5mvdtskz6tkzogTCdKWJv4k12j2BOHq3okVzlZiIsQhQXSh0I86ggUPPf","sha384-zBJRQUocgzK6hCN4Er9zN2l2fljUYdgHRSFXhzsgHwU2/HxnMPRr50a9Uevh/DBF")
                .SetVersion("1.10.20");

            manifest
                .DefineStyle(JQuery_DataTables_Bootstrap4)
                .SetDependencies(Bootstrap)
                .SetUrl("~/Lombiq.DataTables/vendors/datatables.net-bs4-css/dataTables.bootstrap4.min.css", "~/Lombiq.DataTables/vendors/datatables.net-bs4-css/dataTables.bootstrap4.css")
                //.SetCdn("https://cdn.datatables.net/1.10.20/css/dataTables.bootstrap4.min.css", "https://cdn.datatables.net/1.10.20/css/dataTables.bootstrap4.css")
                //.SetCdnIntegrity("sha384-EkHEUZ6lErauT712zSr0DZ2uuCmi3DoQj6ecNdHQXpMpFNGAQ48WjfXCE5n20W+R","sha384-0gIBab94tmRrgNHxYeuwE1hLc+W4Mv5SHxZeETbpK8TdKQlYOUwL0xcEgyw/Yc9U")
                .SetVersion("1.10.20");

            // Custom resources.
            manifest
                .DefineScript(JQuery_DataTables_AutoInit)
                .SetDependencies(JQuery_DataTables_Bootstrap4)
                .SetUrl("~/Lombiq.DataTables/lombiq/jquery-datatables-autoinit/jquery-datatables-autoinit.js")
                .SetVersion("1.0");

            manifest
                .DefineScript(Lombiq_DataTables)
                .SetDependencies(JQuery, Uri_Js, JQuery_DataTables, JQuery_DataTables_Bootstrap4)
                .SetUrl("~/Lombiq.DataTables/lombiq/lombiq-datatables/lombiq-datatables.js")
                .SetVersion("1.0");

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
