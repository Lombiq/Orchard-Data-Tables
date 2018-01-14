using Orchard.Themes;
using System.Web.Mvc;

namespace Lombiq.DataTables.Controllers
{
    [Themed]
    public class DataTablesTestController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}