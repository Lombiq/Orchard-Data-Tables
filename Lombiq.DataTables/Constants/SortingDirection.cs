using System.Runtime.Serialization;

namespace Lombiq.DataTables.Constants;

public enum SortingDirection
{
    [EnumMember(Value = "ascending")]
    Ascending = 0,

    [EnumMember(Value = "descending")]
    Descending,
}
