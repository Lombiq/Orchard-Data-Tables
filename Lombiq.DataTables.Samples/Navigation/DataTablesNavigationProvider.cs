using Lombiq.DataTables.Controllers;
using Lombiq.DataTables.Samples.Controllers;
using Lombiq.DataTables.Samples.Services;
using Lombiq.HelpfulLibraries.OrchardCore.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Lombiq.DataTables.Samples.Navigation;

public class DataTablesNavigationProvider(
    IHttpContextAccessor hca,
    IStringLocalizer<DataTablesNavigationProvider> stringLocalizer) : MainMenuNavigationProviderBase(hca, stringLocalizer)
{
    protected override void Build(NavigationBuilder builder) =>
        builder
            .Add(T["Data Tables"], builder => builder
                .Add(T["Tag Helper"], itemBuilder => itemBuilder
                    .ActionTask<SampleController>(_hca.HttpContext, controller => controller.DataTableTagHelper()))
                .Add(T["JSON Provider"], itemBuilder => itemBuilder
                    .Action<SampleController>(_hca.HttpContext, controller => controller.ProviderWithShape()))
                .Add(T["JSON-based Provider (Admin)"], AdminTable(nameof(SampleJsonResultDataTableDataProvider)))
                .Add(T["Index-based Provider (Admin)"], AdminTable(nameof(SampleIndexBasedDataTableDataProvider))));

    [SuppressMessage(
        "Style",
        "MA0003:Add argument name to improve readability",
        Justification = "You can't use named arguments in Expressions.")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Not unnecessary.")]
    private Action<NavigationItemBuilder> AdminTable(string name) =>
        itemBuilder => itemBuilder
            .ActionTask<TableController>(_hca.HttpContext, controller => controller.Get(
                name,
                null,
                true,
                false));
}
