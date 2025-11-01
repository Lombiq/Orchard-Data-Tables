using Lombiq.DataTables.Constants;
using Lombiq.HelpfulLibraries.Attributes;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.DataTables;

[LibManVersions]
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
            .SetUrl(Vendors + "urijs/URI.min.js", Vendors + "urijs/URI.js")
            .SetDependencies(ResourceNames.JQuery)
            .SetVersion(LibMan_urijs);

        _manifest
            .DefineScript(ResourceNames.DataTables.Library)
            .SetDependencies(ResourceNames.JQuery)
            .SetUrl(
                Vendors + "datatables.net/dataTables.min.js",
                Vendors + "datatables.net/dataTables.js")
            .SetCdn(
                $"https://cdn.datatables.net/{LibMan_datatables_net}/dataTables.min.js",
                $"https://cdn.datatables.net/{LibMan_datatables_net}/dataTables.js")
            .SetVersion(LibMan_datatables_net);

        _manifest
            .DefineScript(ResourceNames.DataTables.Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-buttons/dataTables.buttons.min.js",
                Vendors + "datatables.net-buttons/dataTables.buttons.js")
            .SetVersion(LibMan_datatables_net_buttons);

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap5)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-bs5-js/dataTables.bootstrap5.min.js",
                Vendors + "datatables.net-bs5-js/dataTables.bootstrap5.js")
            .SetCdn(
                $"https://cdn.datatables.net/{LibMan_datatables_net_bs5}/js/dataTables.bootstrap5.min.js",
                $"https://cdn.datatables.net/{LibMan_datatables_net_bs5}/js/dataTables.bootstrap5.js")
            .SetVersion(LibMan_datatables_net_bs5);

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap5)
            .SetDependencies(ResourceNames.Bootstrap)
            .SetUrl(
                Vendors + "datatables.net-bs5-css/dataTables.bootstrap5.min.css",
                Vendors + "datatables.net-bs5-css/dataTables.bootstrap5.css")
            .SetCdn(
                $"https://cdn.datatables.net/{LibMan_datatables_net_bs5}/css/dataTables.bootstrap5.min.css",
                $"https://cdn.datatables.net/{LibMan_datatables_net_bs5}/css/dataTables.bootstrap5.css")
            .SetVersion(LibMan_datatables_net_bs5);

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap5Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-bs5-js/buttons.bootstrap5.min.js",
                Vendors + "datatables.net-bs5-js/buttons.bootstrap5.js")
            .SetVersion(LibMan_datatables_net_buttons_bs5);

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap5Buttons)
            .SetDependencies(ResourceNames.Bootstrap, ResourceNames.DataTables.Bootstrap5, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-bs5-css/buttons.bootstrap5.min.css",
                Vendors + "datatables.net-bs5-css/buttons.bootstrap5.css")
            .SetVersion(LibMan_datatables_net_buttons_bs5);

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
