namespace Lombiq.DataTables.Constants
{
    public static class DataTableColumnDefinitionSettingsKeys
    {
        public static class ContentFieldSorting
        {
            private const string Prefix = nameof(ContentFieldSorting) + ".";

            public const string PropertyName = Prefix + nameof(PropertyName);
            public const string ValueType = Prefix + nameof(ValueType);
        }

        public static class TaxonomyTermSorting
        {
            private const string Prefix = nameof(TaxonomyTermSorting) + ".";

            public const string FieldName = Prefix + nameof(FieldName);
        }
    }
}