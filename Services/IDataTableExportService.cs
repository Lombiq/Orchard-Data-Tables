using System.IO;
using System.Threading.Tasks;
using Lombiq.DataTables.Models;

namespace Lombiq.DataTables.Services
{
    public interface IDataTableExportService
    {
        public const string OctetStreamContentType = "application/octet-stream";

        string Name => GetType().Name;
        string ContentType => OctetStreamContentType;

        string DefaultFileName { get; }

        Task<Stream> ExportAsync(
            IDataTableDataProvider dataProvider,
            DataTableDataRequest request,
            DataTableColumnsDefinition columnsDefinition = null);
    }
}
