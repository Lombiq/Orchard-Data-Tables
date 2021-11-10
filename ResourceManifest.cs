using Orchard.UI.Resources;
using static Lombiq.DataTables.Constants.ResourceNames;

namespace Lombiq.DataTables
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript("jQuery")
                .SetUrl("jquery-migrate.min.js", "jquery-migrate.js")
                .SetVersion("3.3.2")
                .SetCdn(
                    "//cdnjs.cloudflare.com/ajax/libs/jquery-migrate/3.3.2/jquery-migrate.min.js",
                    "//cdnjs.cloudflare.com/ajax/libs/jquery-migrate/3.3.2/jquery-migrate.js",
                    true)
                .SetDependencies("jQueryMigrate");

            manifest.DefineScript("jQueryMigrate")
                .SetUrl("jquery.min.js", "jquery.js")
                .SetVersion("3.6.0")
                .SetCdn(
                    "//cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.min.js",
                    "//cdnjs.cloudflare.com/ajax/libs/jquery/3.6.0/jquery.js",
                    true);

            // jQuery.DataTables-related resources.
            manifest.DefineScript(Uri_Js).SetUrl("../Content/URI.js/URI.min.js", "../Content/URI.js/URI.js").SetDependencies("jQuery");
            manifest.DefineScript(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.js", "../Content/DataTables/dataTables.js").SetDependencies("jQuery");
            manifest.DefineScript(JQuery_DataTables_Plugins_Processing).SetUrl("../Content/DataTables/Plugins/processing().js").SetDependencies(JQuery_DataTables);
            manifest.DefineScript(JQuery_DataTables_Extensions_Responsive).SetUrl("../Content/DataTables/Extensions/Responsive/js/dataTables.responsive.min.js", "../Content/DataTables/Extensions/Responsive/js/dataTables.responsive.js").SetDependencies(JQuery_DataTables);
            manifest.DefineScript(JQuery_DataTables_Extensions_ColReorder).SetUrl("../Content/DataTables/Extensions/ColReorder/js/dataTables.colReorder.min.js").SetDependencies(JQuery_DataTables);

            manifest.DefineStyle(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.css", "../Content/DataTables/dataTables.css");
            manifest.DefineStyle(JQuery_DataTables_Extensions_Responsive).SetUrl("../Content/DataTables/Extensions/Responsive/css/responsive.dataTables.min.css").SetDependencies(JQuery_DataTables);
            manifest.DefineStyle(JQuery_DataTables_Extensions_ColReorder).SetUrl("../Content/DataTables/Extensions/ColReorder/css/colReorder.dataTables.min.css").SetDependencies(JQuery_DataTables);

            // Custom resources.
            manifest.DefineScript(Lombiq_DataTables).SetUrl("lombiq-datatables.js").SetDependencies(Uri_Js, JQuery_DataTables_Plugins_Processing, JQuery_DataTables_Extensions_ColReorder);
            manifest.DefineScript(Lombiq_ContentPicker).SetUrl("lombiq-contentpicker.js").SetDependencies("jQueryColorBox");

            manifest.DefineStyle(Lombiq_ContentPicker).SetDependencies("jQueryColorBox");
        }
    }
}