namespace Lombiq.DataTables.Models
{
    public class VueModelCheckbox
    {
        public string Type => "checkbox";
        public string Name { get; set; }
        public string Text { get; set; }
        public bool? Value { get; set; }
        public string Classes { get; set; } = string.Empty;
    }
}
