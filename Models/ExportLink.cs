﻿namespace Lombiq.DataTables.Models
{
    public class ExportLink
    {
        public string Type { get => nameof(ExportLink); set {} }
        public string Url { get; set; }
        public string Text { get; set; }

        public ExportLink() { }

        public ExportLink(string url, string text)
        {
            Url = url;
            Text = text;
        }
    }
}
