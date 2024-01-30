using Lombiq.DataTables.Samples.Models;
using Lombiq.HelpfulLibraries.OrchardCore.Contents;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using System.Linq;
using System.Threading.Tasks;
using YesSql;
using static Lombiq.DataTables.Samples.Constants.ContentTypes;

namespace Lombiq.DataTables.Samples.Controllers;

public class SampleController : Controller
{
    private readonly ISession _session;

    public SampleController(ISession session) => _session = session;

    // The tag helper needs the data during page render, which can make the initial page load slower. This is okay for
    // small tables. You'll see approaches better suited for large datasets later. See it under
    // /Lombiq.DataTables.Samples/Sample/DataTableTagHelper
    public async Task<IActionResult> DataTableTagHelper() =>
        View(
            (await _session
                .QueryContentItem(PublicationStatus.Published)
                .Where(index => index.ContentType == Employee)
                .ListAsync())
            .Select(contentItem => contentItem.As<EmployeePart>()));

    // Nothing interesting happens here, the shape sends out the asynchronous request on its own via JavaScript. See it
    // under /Lombiq.DataTables.Samples/Sample/ProviderWithShape
    public IActionResult ProviderWithShape() => View();
}
