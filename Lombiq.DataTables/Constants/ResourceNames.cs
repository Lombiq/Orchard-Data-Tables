namespace Lombiq.DataTables.Constants;

public static class ResourceNames
{
    // These are registered in OrchardCore.Resources.
    public const string JQuery = "jQuery";

    public const string Bootstrap = "bootstrap";

    public const string UriJs = nameof(UriJs);
    public const string LombiqDataTables = nameof(LombiqDataTables);
    public const string LombiqDataTablesBefore = nameof(LombiqDataTablesBefore);
    public const string ICantBelieveItsNotDataTable = nameof(ICantBelieveItsNotDataTable);

    public static class DataTables
    {
        public const string Library = nameof(DataTables);

        public const string Buttons = Library + nameof(Buttons);
        public const string Bootstrap5 = Library + nameof(Bootstrap5);
        public const string Bootstrap5Buttons = Library + nameof(Bootstrap5Buttons);
        public const string AutoInit = Library + nameof(AutoInit);
    }
}
