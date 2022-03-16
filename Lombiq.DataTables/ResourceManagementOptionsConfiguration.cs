using Lombiq.DataTables.Constants;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Lombiq.DataTables;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
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
            .SetUrl(Vendors + "urijs/URI.min.js", Vendors + "urijs/URI.js")
            .SetDependencies(ResourceNames.JQuery)
            .SetVersion("1.19.2");

        _manifest
            .DefineScript(ResourceNames.DataTables.Library)
            .SetDependencies(ResourceNames.JQuery)
            .SetUrl(
                Vendors + "datatables.net/jquery.dataTables.min.js",
                Vendors + "datatables.net/jquery.dataTables.js")
            .SetCdn(
                "https://cdn.datatables.net/1.10.20/js/jquery.dataTables.min.js",
                "https://cdn.datatables.net/1.10.20/js/jquery.dataTables.js")
            .SetCdnIntegrity(
                "sha384-L74JDRkaoB7PWnReNepwX6+kSckc13TJXrka4EerY9jxQxSDl0dTguSLcA7dEfq8",
                "sha384-FeGVmTD/nb8R8suJjHKxL3iAigW2uFc536mNbyfM60EY3KH6wit0Jmgx0/QO2reU")
            .SetVersion("1.10.20");

        _manifest
            .DefineScript(ResourceNames.DataTables.Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-buttons/dataTables.buttons.min.js",
                Vendors + "datatables.net-buttons/dataTables.buttons.js")
            .SetVersion("1.6.3");

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap4)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library)
            .SetUrl(
                Vendors + "datatables.net-bs4-js/dataTables.bootstrap4.min.js",
                Vendors + "datatables.net-bs4-js/dataTables.bootstrap4.js")
            .SetCdn(
                "https://cdn.datatables.net/1.10.20/js/dataTables.bootstrap4.min.js",
                "https://cdn.datatables.net/1.10.20/js/dataTables.bootstrap4.js")
            .SetCdnIntegrity(
                "sha384-dsXH1jw5mvdtskz6tkzogTCdKWJv4k12j2BOHq3okVzlZiIsQhQXSh0I86ggUPPf",
                "sha384-zBJRQUocgzK6hCN4Er9zN2l2fljUYdgHRSFXhzsgHwU2/HxnMPRr50a9Uevh/DBF")
            .SetVersion("1.10.20");

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap4)
            .SetDependencies(ResourceNames.Bootstrap)
            .SetUrl(
                Vendors + "datatables.net-bs4-css/dataTables.bootstrap4.min.css",
                Vendors + "datatables.net-bs4-css/dataTables.bootstrap4.css")
            .SetCdn(
                "https://cdn.datatables.net/1.10.20/css/dataTables.bootstrap4.min.css",
                "https://cdn.datatables.net/1.10.20/css/dataTables.bootstrap4.css")
            .SetCdnIntegrity(
                "sha384-EkHEUZ6lErauT712zSr0DZ2uuCmi3DoQj6ecNdHQXpMpFNGAQ48WjfXCE5n20W+R",
                "sha384-0gIBab94tmRrgNHxYeuwE1hLc+W4Mv5SHxZeETbpK8TdKQlYOUwL0xcEgyw/Yc9U")
            .SetVersion("1.10.20");

        _manifest
            .DefineScript(ResourceNames.DataTables.Bootstrap4Buttons)
            .SetDependencies(ResourceNames.JQuery, ResourceNames.DataTables.Library, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-bs4-js/buttons.bootstrap4.min.js",
                Vendors + "datatables.net-bs4-js/buttons.bootstrap4.js")
            .SetVersion("1.6.3");

        _manifest
            .DefineStyle(ResourceNames.DataTables.Bootstrap4Buttons)
            .SetDependencies(ResourceNames.Bootstrap, ResourceNames.DataTables.Bootstrap4, ResourceNames.DataTables.Buttons)
            .SetUrl(
                Vendors + "datatables.net-bs4-css/buttons.bootstrap4.min.css",
                Vendors + "datatables.net-bs4-css/buttons.bootstrap4.css")
            .SetVersion("1.6.3");

        // Custom resources.
        _manifest
            .DefineScript(ResourceNames.DataTables.AutoInit)
            .SetDependencies(ResourceNames.DataTables.Bootstrap4, ResourceNames.DataTables.Bootstrap4Buttons)
            .SetUrl(Lombiq + "jquery-datatables-autoinit.js")
            .SetVersion("1.0");

        _manifest
            .DefineScript(ResourceNames.LombiqDataTables)
            .SetDependencies(
                ResourceNames.JQuery,
                ResourceNames.UriJs,
                ResourceNames.DataTables.Library,
                ResourceNames.DataTables.Bootstrap4,
                ResourceNames.DataTables.Buttons,
                ResourceNames.DataTables.Bootstrap4Buttons)
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
