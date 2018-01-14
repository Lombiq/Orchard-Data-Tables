using System.Runtime.Serialization;

namespace Lombiq.DataTables.Constants
{
    public enum SortingDirection
    {
        [EnumMember(Value = "asc")]
        Asc = 0,

        [EnumMember(Value = "desc")]
        Desc
    }
}