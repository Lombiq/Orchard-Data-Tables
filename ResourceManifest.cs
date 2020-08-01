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
                .SetDependencies("jQuery");
            manifest
                .DefineScript(JQuery_DataTables)
                .SetUrl("~/Lombiq.DataTables/Content/DataTables/dataTables.min.js", "~/Lombiq.DataTables/Content/DataTables/dataTables.js")
                .SetDependencies("jQuery");
            manifest
                .DefineScript(JQuery_DataTables_Plugins_Processing)
                .SetUrl("~/Lombiq.DataTables/Content/DataTables/Plugins/processing().js")
                .SetDependencies(JQuery_DataTables);
            manifest
                .DefineScript(JQuery_DataTables_Extensions_Responsive)
                .SetUrl("~/Lombiq.DataTables/Content/DataTables/Extensions/Responsive/js/dataTables.responsive.min.js", "~/Lombiq.DataTables/Content/DataTables/Extensions/Responsive/js/dataTables.responsive.js")
                .SetDependencies(JQuery_DataTables);

            manifest
                .DefineStyle(JQuery_DataTables)
                .SetUrl("~/Lombiq.DataTables/Content/DataTables/dataTables.min.css", "~/Lombiq.DataTables/Content/DataTables/dataTables.css");
            manifest
                .DefineStyle(JQuery_DataTables_Extensions_Responsive)
                .SetUrl("~/Lombiq.DataTables/Content/DataTables/Extensions/Responsive/css/responsive.dataTables.min.css")
                .SetDependencies(JQuery_DataTables);

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
