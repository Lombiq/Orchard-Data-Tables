namespace Lombiq.DataTables.Constants
{
    public static class DataTableRoutes
    {
        private const string RouteBase = "/Admin/DataTable";

        public const string GetDataTableData = RouteBase + "/{providerName}/{queryId?}";
    }
}
