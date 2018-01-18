using Newtonsoft.Json;

namespace Lombiq.DataTables.Models
{
    public class DataTableChildRowResponse
    {
        [JsonProperty(PropertyName = "error")]
        public string Error { get; set; }

        [JsonProperty(PropertyName = "content")]
        public string Content { get; set; }


        public static DataTableChildRowResponse ErrorResult(string errorText) =>
            new DataTableChildRowResponse { Error = errorText };
    }
}