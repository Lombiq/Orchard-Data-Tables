using Microsoft.AspNetCore.Mvc;

namespace Lombiq.DataTables.Samples.Controllers
{
    // This controller doesn't do anything interesting, check out the views to see the relevant parts.
    public class SampleController : Controller
    {
        public IActionResult DataTableTagHelper() => View();
        public IActionResult ProviderWithShape() => View();
    }
}
