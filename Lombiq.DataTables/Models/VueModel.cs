using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Routing;
using OrchardCore.ContentManagement;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Lombiq.DataTables.Models;

public class VueModel
{
    /// <summary>
    /// Gets or sets the text to be displayed. Always fill this unless you have <see cref="Html"/> or plan to use the
    /// <c>component</c> property client-side. Even then, you need to provide either this or <see cref="Sort"/> if the
    /// column is meant to be <see cref="DataTableColumnDefinition.Orderable"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("text")]
    public string Text { get; set; }

    /// <summary>
    /// Gets or sets the HTML content to be rendered inside the cell. When used <see cref="Text"/> and <see
    /// cref="Badge"/> are ignored.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("html")]
    public string Html { get; set; }

    /// <summary>
    /// Gets or sets the value used for sorting. If <see langword="null"/> or empty, the value of <see cref="Text"/> is
    /// used for sorting.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("sort")]
    public object Sort { get; set; }

    /// <summary>
    /// Gets or sets the URL to be used in the <c>href</c> attributes. When this is used <see cref="Html"/> is ignored.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("href")]
    public string Href { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("multipleLinks")]
    public IEnumerable<MultipleHrefValue> MultipleLinks { get; set; }

    /// <summary>
    /// Gets or sets the Bootstrap badge class of the cell. To be used along with <see cref="Text"/> and optionally <see
    /// cref="Href"/>.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("badge")]
    public object Badge { get; set; }

    /// <summary>
    /// Gets or sets the data used as extra information to be consumed by <c>special</c> event so the contents can be
    /// updated with JavaScript on client side before each render.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("special")]
    public object Special { get; set; }

    /// <summary>
    /// Gets or sets the value of an <c>&lt;input type="hidden"&gt;</c> element that's rendered after the table (so
    /// unaffected by paging and sorting) which can contain data to be POSTed back to the server if the table is inside
    /// a <c>form</c> element.
    /// </summary>
    [JsonIgnore]
    public HiddenInputValue HiddenInput { get; set; }

    /// <summary>
    /// Gets or sets the <c>Array</c> value of <c>hiddenInput</c>. Same as <see cref="HiddenInput"/>, except it contains
    /// multiple instances in case the same cell needs to render several hidden <c>input</c> elements.
    /// </summary>
    [JsonIgnore]
    public IEnumerable<HiddenInputValue> HiddenInputs { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    [JsonPropertyName("hiddenInput")]
    [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "It's used by STJ.")]
    private object HiddenInputSerialize
    {
        get => (object)HiddenInput ?? HiddenInputs;
        set
        {
            switch (value)
            {
                case JsonArray hiddenInputs:
                    HiddenInputs = hiddenInputs.ToObject<IEnumerable<HiddenInputValue>>();
                    break;
                case JsonObject hiddenInput:
                    HiddenInput = hiddenInput.ToObject<HiddenInputValue>();
                    break;
                case null:
                    return;
                default:
                    throw new InvalidCastException("The value of \"hiddenInput\" must be Object, Array or null.");
            }
        }
    }

    public VueModel(IContent content, bool isEditor, IUrlHelper urlHelper)
    {
        if (content == null) return;

        Text = content.ContentItem.DisplayText;
        Href = isEditor
            ? urlHelper.EditContentItem(content.ContentItem.ContentItemId)
            : urlHelper.DisplayContentItem(content);
    }

    public VueModel(string text = null, string href = null, object sort = null)
    {
        Text = text;
        Href = href;
        Sort = sort;
    }

    public VueModel(int number, string href = null)
        : this(number.ToString(CultureInfo.InvariantCulture), href, number)
    {
    }

    public VueModel SetHiddenInput(string name, string value)
    {
        HiddenInput = new HiddenInputValue { Name = name, Value = value };
        return this;
    }

    public static async Task<JsonArray> TableToJsonAsync<T>(
        IEnumerable<T> collection,
        Func<T, int, Task<Dictionary<string, VueModel>>> select)
    {
        var rows = (await collection.AwaitEachAsync(select))
            .Select((row, rowIndex) =>
            {
                var castRow = row.ToDictionary(pair => pair.Key, pair => JsonSerializer.SerializeToNode(pair.Value));
                castRow["$rowIndex"] = rowIndex;
                return castRow;
            });

        return JArray.FromObject(rows);
    }

    public static IDictionary<string, string> CreateTextForIcbinDataTable(IHtmlLocalizer localizer) =>
        new Dictionary<string, string>
        {
            ["lengthPicker"] = localizer["Show {{ count }} Entries"].Value,
            ["displayCount"] = localizer["Showing {{ from }} to {{ to }} of {{ total }} entries"].Value,
            ["previous"] = localizer["Previous"].Value,
            ["next"] = localizer["Next"].Value,
            ["all"] = localizer["All"].Value,
        };

    public class HiddenInputValue
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }
    }

    public class MultipleHrefValue
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public static class BadgeNames
    {
        public const string Primary = "primary";
        public const string Secondary = "secondary";
        public const string Success = "success";
        public const string Danger = "danger";
        public const string Warning = "warning";
        public const string Info = "info";
        public const string Light = "light";
        public const string Dark = "dark";
    }
}
