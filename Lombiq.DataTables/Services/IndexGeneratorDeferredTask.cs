using Lombiq.DataTables.Handlers;
using Lombiq.HelpfulLibraries.AspNetCore.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services;

public class IndexGeneratorDeferredTask(IEnumerable<IManualDataTableIndexGenerator> manualDataTableIndexGenerators) : IDeferredTask
{
    public bool IsScheduled { get; set; }

    public Task PreProcessAsync(HttpContext context)
    {
        foreach (var generator in manualDataTableIndexGenerators)
        {
            generator.IsInMiddlewarePipeline = true;
        }

        return Task.CompletedTask;
    }

    public Task PostProcessAsync(HttpContext context) =>
        manualDataTableIndexGenerators.GenerateOrderedIndicesAsync();
}
