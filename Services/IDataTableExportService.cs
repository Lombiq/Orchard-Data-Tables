using System.IO;
using System.Threading.Tasks;
using Lombiq.DataTables.Models;

namespace Lombiq.DataTables.Services
{
    /// <summary>
    /// A service to send a <see cref="DataTableDataRequest"/> into the <see cref="IDataTableDataProvider"/> and export
    /// the results as file into a <see cref="Stream"/>.
    /// </summary>
    public interface IDataTableExportService
    {
        /// <summary>
        /// The default content type that tells the browser to just download the response.
        /// </summary>
        public const string OctetStreamContentType = "application/octet-stream";

        /// <summary>
        /// The technical name used to identify the service.
        /// </summary>
        string Name => GetType().Name;

        /// <summary>
        /// The suggested content-type header when returning the file as an HTTP response.
        /// </summary>
        string ContentType => OctetStreamContentType;

        /// <summary>
        /// The suggested file name when returning the file as an HTTP response.
        /// </summary>
        string DefaultFileName { get; }

        /// <summary>
        /// Requests the data from the provider and exports it into the desired file in stream.
        /// </summary>
        /// <param name="dataProvider">Gets the dataset.</param>
        /// <param name="request">Configures the <see cref="dataProvider"/>.</param>
        /// <param name="columnsDefinition">Contains the list of columns to export.</param>
        /// <returns>The file serialized into a stream.</returns>
        /// <remarks>The <see cref="DataTableColumnDefinition.Exportable"/> is relevant here.</remarks>
        Task<Stream> ExportAsync(
            IDataTableDataProvider dataProvider,
            DataTableDataRequest request,
            DataTableColumnsDefinition columnsDefinition = null);
    }
}
