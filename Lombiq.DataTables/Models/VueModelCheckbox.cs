using Newtonsoft.Json;

namespace Lombiq.DataTables.Models;

// This class is used in <icbin-datatable>.
public class VueModelCheckbox
{
    [JsonProperty("type")]
    public string Type => "checkbox";

    [JsonProperty("name")]
    public string Name { get; set; }

    [JsonProperty("text")]
    public string Text { get; set; }

    [JsonProperty("value")]
    public bool? Value { get; set; }

    [JsonProperty("classes")]
    public string Classes { get; set; } = string.Empty;
}
