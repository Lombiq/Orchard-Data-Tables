using Lombiq.DataTables.Handlers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Middlewares
{
    internal class IndexGeneratorMiddleware
    {
        private readonly RequestDelegate _next;

        public IndexGeneratorMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(
            HttpContext context,
            IEnumerable<IManualDataTableIndexGenerator> manualDataTableIndexGenerators)
        {
            foreach (var generator in manualDataTableIndexGenerators) generator.IsInMiddlewarePipeline = true;

            await _next(context);

            // Code is after _next(context) so it is executed on the return trip to ensure it's one of the very last
            // things in every request.
            await manualDataTableIndexGenerators.GenerateOrderedIndicesAsync();
        }
    }
}
