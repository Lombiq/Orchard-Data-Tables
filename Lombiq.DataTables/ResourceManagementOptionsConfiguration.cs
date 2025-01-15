using Lombiq.DataTables.Constants;
using Lombiq.HelpfulLibraries.Attributes;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.DataTables;

[ConstantFromJson("UriJsVersion", "package.json", "urijs")] // #spell-check-ignore-line
[ConstantFromJson("DataTablesVersion", "package.json", "datatables.net")]
[ConstantFromJson("DataTablesBootstrap5Version", "package.json", "datatables.net-bs5")]
[ConstantFromJson("DataTablesButtonsVersion", "package.json", "datatables.net-buttons")]
[ConstantFromJson("DataTablesButtonsBootstrap5Version", "package.json", "datatables.net-buttons-bs5")]
public partial class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private const string WwwRoot = "~/Lombiq.DataTables/";
    private const string Vendors = WwwRoot + "vendors/";
    private const string Lombiq = WwwRoot + "lombiq/";
    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        // jQuery.DataTables-related resources.
        _manifest
            .DefineScript(ResourceNames.UriJs)
            .SetUrl(Vendors + "urijs/URI.min.js", Vendors + "urijs/URI.js") // #spell-check-ignore-line
            .SetDependencies(ResourceNames.JQuery)
            .SetVersion(UriJsVersion);

        _manifest
            .DefineScript(ResourceNames.DataTables.Library)
            .SetDependencies(ResourceNames.JQuery)
            .SetUrl(
                Vendors + "datatables.net/dataTables.min.js",
                Vendors + "datatables.net/dataTables.js")
            .SetCdn(
                "https://cdn.datatables.net/2.2.1/js/dataTables.min.js",
                "https://cdn.datatables.net/2.2.1/js/dataTables.js")
            .SetCdnIntegrity(
                "sha384-L74JDRkaoB7PWnReNepwX6+kSckc13TJXrka4EerY9jxQxSDl0dTguSLcA7dEfq8",
                "sha384-FeGVmTD/nb8R8suJjHKxL3iAigW2uFc536mNbyfM60EY3KH6wit0Jmgx0/QO2reU")
            .SetVersion(DataTablesVersion);

        _manifest
            .DefineScript(ResourceNames.DataTables.Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-buttons/dataTables.buttons.min.js",
                Vendors + "datatables.net-buttons/dataTables.buttons.js")
            .SetVersion(DataTablesButtonsVersion);

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap5)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-bs5-js/dataTables.bootstrap5.min.js",
                Vendors + "datatables.net-bs5-js/dataTables.bootstrap5.js")
            .SetCdn(
                "https://cdn.datatables.net/2.2.1/js/dataTables.bootstrap5.min.js",
                "https://cdn.datatables.net/2.2.1/js/dataTables.bootstrap5.js")
            .SetCdnIntegrity(
                "sha384-dsXH1jw5mvdtskz6tkzogTCdKWJv4k12j2BOHq3okVzlZiIsQhQXSh0I86ggUPPf",
                "sha384-zBJRQUocgzK6hCN4Er9zN2l2fljUYdgHRSFXhzsgHwU2/HxnMPRr50a9Uevh/DBF")
            .SetVersion(DataTablesBootstrap5Version);

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap5)
            .SetDependencies(ResourceNames.Bootstrap)
            .SetUrl(
                Vendors + "datatables.net-bs5-css/dataTables.bootstrap5.min.css",
                Vendors + "datatables.net-bs5-css/dataTables.bootstrap5.css")
            .SetCdn(
                "https://cdn.datatables.net/2.2.1/css/dataTables.bootstrap5.min.css",
                "https://cdn.datatables.net/2.2.1/css/dataTables.bootstrap5.css")
            .SetCdnIntegrity(
                "sha384-EkHEUZ6lErauT712zSr0DZ2uuCmi3DoQj6ecNdHQXpMpFNGAQ48WjfXCE5n20W+R",
                "sha384-0gIBab94tmRrgNHxYeuwE1hLc+W4Mv5SHxZeETbpK8TdKQlYOUwL0xcEgyw/Yc9U")
            .SetVersion(DataTablesBootstrap5Version);

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap5Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-bs5-js/buttons.bootstrap5.min.js",
                Vendors + "datatables.net-bs5-js/buttons.bootstrap5.js")
            .SetVersion(DataTablesButtonsBootstrap5Version);

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap5Buttons)
            .SetDependencies(ResourceNames.Bootstrap, ResourceNames.DataTables.Bootstrap5, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-bs5-css/buttons.bootstrap5.min.css",
                Vendors + "datatables.net-bs5-css/buttons.bootstrap5.css")
            .SetVersion(DataTablesButtonsBootstrap5Version);

        // Custom resources.
        _manifest
            .DefineScript(ResourceNames.DataTables.AutoInit)
            .SetDependencies(ResourceNames.DataTables.Bootstrap5, ResourceNames.DataTables.Bootstrap5Buttons)
            .SetUrl(Lombiq + "jquery-datatables-autoinit.js")
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
            .SetUrl(Lombiq + "lombiq-datatables.js")
            .SetVersion("1.0");

        _manifest
            .DefineScript(ResourceNames.ICantBelieveItsNotDataTable)
            .SetUrl(Lombiq + "icbin-datatable.js")
            .SetDependencies("vuejs")
            .SetVersion("1.0.2");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
