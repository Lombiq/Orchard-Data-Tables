using Lombiq.DataTables.Handlers;
using Lombiq.HelpfulLibraries.Libraries.Middlewares;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Services
{
    public class IndexGeneratorDeferredTask : IDeferredTask
    {
        private readonly IEnumerable<IManualDataTableIndexGenerator> _manualDataTableIndexGenerators;
        public bool IsScheduled { get; set; }

        public IndexGeneratorDeferredTask(IEnumerable<IManualDataTableIndexGenerator> manualDataTableIndexGenerators) =>
            _manualDataTableIndexGenerators = manualDataTableIndexGenerators;

        public Task PreProcessAsync(HttpContext context)
        {
            foreach (var generator in _manualDataTableIndexGenerators)
            {
                generator.IsInMiddlewarePipeline = true;
            }

            return Task.CompletedTask;
        }

        public Task PostProcessAsync(HttpContext context) =>
            _manualDataTableIndexGenerators.GenerateOrderedIndicesAsync();
    }
}
