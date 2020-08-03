using System.IO;
using System.Threading.Tasks;
using Lombiq.DataTables.Models;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableExportService
    {
        Task<Stream> ExportAsync(
            IDataTableDataProvider dataProvider,
            DataTableDataRequest request,
            DataTableColumnsDefinition columnsDefinition = null);
    }
}
