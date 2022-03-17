using Lombiq.DataTables.Services;
using System.Linq;

namespace System.Collections.Generic;

public static class EnumerableExtensions
{
    public static IDataTableDataProvider GetDataProvider(this IEnumerable<IDataTableDataProvider> providers, string name) =>
        providers.FirstOrDefault(provider => provider.Name == name);
}
