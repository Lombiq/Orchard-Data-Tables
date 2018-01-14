using Orchard.UI.Resources;
using static Lombiq.DataTables.Constants.ResourceNames;

namespace Lombiq.DataTables
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.js", "../Content/DataTables/dataTables.js").SetDependencies("JQuery");
            manifest.DefineScript(Lombiq_DataTables).SetUrl("lombiq-datatables.js").SetDependencies(JQuery_DataTables);

            manifest.DefineStyle(JQuery_DataTables).SetUrl("../Content/DataTables/dataTables.min.css", "../Content/DataTables/dataTables.css");
        }
    }
}