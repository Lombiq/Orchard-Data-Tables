using Orchard.UI.Resources;
using static Lombiq.DataTables.Constants.ResourceNames;

namespace Lombiq.DataTables
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            // jQuery.DataTables-related resources.
            manifest.DefineScript(Uri_Js).SetUrl("../Content/URI.js/URI.min.js", "../Content/URI.js/URI.js").SetDependencies("jQuery");
            manifest.DefineScript(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.js", "../Content/DataTables/dataTables.js").SetDependencies("jQuery");
            manifest.DefineScript(JQuery_DataTables_Plugins_Processing).SetUrl("../Content/DataTables/Plugins/processing().js").SetDependencies(JQuery_DataTables);
            manifest.DefineScript(JQuery_DataTables_Plugins_DateTime_Moment).SetUrl("../Content/DataTables/Plugins/datetime-moment.js").SetCdn("//cdn.datatables.net/plug-ins/1.11.3/sorting/datetime-moment.js").SetVersion("1.11.3").SetDependencies(JQuery_DataTables, "Moment");
            manifest.DefineScript(JQuery_DataTables_Extensions_Responsive).SetUrl("../Content/DataTables/Extensions/Responsive/js/dataTables.responsive.min.js", "../Content/DataTables/Extensions/Responsive/js/dataTables.responsive.js").SetDependencies(JQuery_DataTables);
            manifest.DefineScript(JQuery_DataTables_Extensions_ColReorder).SetUrl("../Content/DataTables/Extensions/ColReorder/js/dataTables.colReorder.min.js").SetDependencies(JQuery_DataTables);

            manifest.DefineStyle(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.css", "../Content/DataTables/dataTables.css");
            manifest.DefineStyle(JQuery_DataTables_Extensions_Responsive).SetUrl("../Content/DataTables/Extensions/Responsive/css/responsive.dataTables.min.css").SetDependencies(JQuery_DataTables);
            manifest.DefineStyle(JQuery_DataTables_Extensions_ColReorder).SetUrl("../Content/DataTables/Extensions/ColReorder/css/colReorder.dataTables.min.css").SetDependencies(JQuery_DataTables);

            // Custom resources.
            manifest.DefineScript(Lombiq_DataTables).SetUrl("lombiq-datatables.js").SetDependencies(Uri_Js, JQuery_DataTables_Plugins_Processing, JQuery_DataTables_Plugins_DateTime_Moment, JQuery_DataTables_Extensions_ColReorder);
            manifest.DefineScript(Lombiq_ContentPicker).SetUrl("lombiq-contentpicker.js").SetDependencies("jQueryColorBox");

            manifest.DefineStyle(Lombiq_ContentPicker).SetDependencies("jQueryColorBox");
        }
    }
}