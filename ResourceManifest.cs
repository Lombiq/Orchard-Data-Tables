using Orchard.UI.Resources;
using static Lombiq.DataTables.Constants.ResourceNames;

namespace Lombiq.DataTables
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript(Uri_Js).SetUrl("../Content/URI.js/URI.min.js", "../Content/URI.js/URI.js").SetDependencies("jQuery");
            manifest.DefineScript(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.js", "../Content/DataTables/dataTables.js").SetDependencies("jQuery");
            manifest.DefineScript(JQuery_DataTables_Processing).SetUrl("../Content/DataTables/Plugins/processing().js").SetDependencies(JQuery_DataTables);
            manifest.DefineScript(Lombiq_DataTables).SetUrl("lombiq-datatables.js").SetDependencies(Uri_Js, JQuery_DataTables_Processing);

            manifest.DefineStyle(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.css", "../Content/DataTables/dataTables.css");
            manifest.DefineStyle(Lombiq_DataTables).SetDependencies(JQuery_DataTables);
        }
    }
}