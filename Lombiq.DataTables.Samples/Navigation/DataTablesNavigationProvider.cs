using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Samples.Controllers;
using Lombiq.DataTables.Samples.Services;
using Lombiq.HelpfulLibraries.OrchardCore.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;

namespace Lombiq.DataTables.Samples.Navigation;

public class DataTablesNavigationProvider : MainMenuNavigationProviderBase
{
    public DataTablesNavigationProvider(
        IHttpContextAccessor hca,
        IStringLocalizer<DataTablesNavigationProvider> stringLocalizer)
        : base(hca, stringLocalizer)
    {
    }

    protected override void Build(NavigationBuilder builder) =>
        builder
            .Add(T["Data Tables"], builder => builder
                .Add(T["Tag Helper"], itemBuilder => itemBuilder
                    .ActionTask<SampleController>(_hca.HttpContext, controller => controller.DataTableTagHelper()))
                .Add(T["JSON Provider"], itemBuilder => itemBuilder
                    .Action<SampleController>(_hca.HttpContext, controller => controller.ProviderWithShape()))
                .Add(T["JSON-based Provider (Admin)"], AdminTable(nameof(SampleJsonResultDataTableDataProvider)))
                .Add(T["Index-based Provider (Admin)"], AdminTable(nameof(SampleIndexBasedDataTableDataProvider))));

    private Action<NavigationItemBuilder> AdminTable(string name) =>
        itemBuilder => itemBuilder.ActionTask<TableController>(
            _hca.HttpContext,
            controller => controller.Get(name, null, true, false));
}
