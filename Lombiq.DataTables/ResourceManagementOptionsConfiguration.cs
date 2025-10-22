using Lombiq.DataTables.Constants;
using Lombiq.HelpfulLibraries.Attributes;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.DataTables;

[ConstantFromJson("UriJsVersion", "package.json", "urijs")]
[ConstantFromJson("DataTablesVersion", "package.json", "datatables.net")]
[ConstantFromJson("DataTablesBootstrap5Version", "package.json", "datatables.net-bs5")]
[ConstantFromJson("DataTablesButtonsVersion", "package.json", "datatables.net-buttons")]
[ConstantFromJson("DataTablesButtonsBootstrap5Version", "package.json", "datatables.net-buttons-bs5")]
public partial class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private const string WwwRoot = "~/Lombiq.DataTables/";
    private const string Vendors = WwwRoot + "vendors/";
    private const string Js = WwwRoot + "js/";
    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        // jQuery.DataTables-related resources.
        _manifest
            .DefineScript(ResourceNames.UriJs)
            .SetUrl(Vendors + "urijs/src/URI.min.js", Vendors + "urijs/src/URI.js")
            .SetDependencies(ResourceNames.JQuery)
            .SetVersion(UriJsVersion);

        _manifest
            .DefineScript(ResourceNames.DataTables.Library)
            .SetDependencies(ResourceNames.JQuery)
            .SetUrl(
                Vendors + "datatables.net/js/dataTables.min.js",
                Vendors + "datatables.net/js/dataTables.js")
            .SetCdn(
                $"https://cdn.datatables.net/{DataTablesVersion}/dataTables.min.js",
                $"https://cdn.datatables.net/{DataTablesVersion}/dataTables.js")
            .SetVersion(DataTablesVersion);

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap5)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-bs5/js/dataTables.bootstrap5.min.js",
                Vendors + "datatables.net-bs5/js/dataTables.bootstrap5.js")
            .SetCdn(
                $"https://cdn.datatables.net/{DataTablesBootstrap5Version}/js/dataTables.bootstrap5.min.js",
                $"https://cdn.datatables.net/{DataTablesBootstrap5Version}/js/dataTables.bootstrap5.js")
            .SetVersion(DataTablesBootstrap5Version);

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap5)
            .SetDependencies(ResourceNames.Bootstrap)
            .SetUrl(
                Vendors + "datatables.net-bs5/css/dataTables.bootstrap5.min.css",
                Vendors + "datatables.net-bs5/css/dataTables.bootstrap5.css")
            .SetCdn(
                $"https://cdn.datatables.net/{DataTablesBootstrap5Version}/css/dataTables.bootstrap5.min.css",
                $"https://cdn.datatables.net/{DataTablesBootstrap5Version}/css/dataTables.bootstrap5.css")
            .SetVersion(DataTablesBootstrap5Version);

        _manifest
            .DefineScript(ResourceNames.DataTables.Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-buttons/js/dataTables.buttons.min.js",
                Vendors + "datatables.net-buttons/js/dataTables.buttons.js")
            .SetVersion(DataTablesButtonsVersion);

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap5Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-buttons-bs5/js/buttons.bootstrap5.min.js",
                Vendors + "datatables.net-buttons-bs5/js/buttons.bootstrap5.js")
            .SetVersion(DataTablesButtonsBootstrap5Version);

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap5Buttons)
            .SetDependencies(ResourceNames.Bootstrap, ResourceNames.DataTables.Bootstrap5, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-buttons-bs5/css/buttons.bootstrap5.min.css",
                Vendors + "datatables.net-buttons-bs5/css/buttons.bootstrap5.css")
            .SetVersion(DataTablesButtonsBootstrap5Version);

        // Custom resources.
        _manifest
            .DefineScript(ResourceNames.DataTables.AutoInit)
            .SetDependencies(ResourceNames.DataTables.Bootstrap5, ResourceNames.DataTables.Bootstrap5Buttons)
            .SetUrl(Js + "jquery-datatables-autoinit.js")
            .SetVersion("1.0");

        _manifest
            .DefineScript(ResourceNames.LombiqDataTables)
            .SetDependencies(
                ResourceNames.JQuery,
                ResourceNames.UriJs,
                ResourceNames.DataTables.Library,
                ResourceNames.DataTables.Bootstrap5,
                ResourceNames.DataTables.Buttons,
                ResourceNames.DataTables.Bootstrap5Buttons)
            .SetUrl(Js + "lombiq-datatables.js")
            .SetVersion("1.0");

        _manifest
            .DefineScript(ResourceNames.ICantBelieveItsNotDataTable)
            .SetUrl(Js + "icbin-datatable.js")
            .SetDependencies("vuejs")
            .SetVersion("1.0.2");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
