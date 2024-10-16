namespace System.Text.Json.Nodes;

public static class JsonNodeExtensions
{
    /// <summary>
    /// Checks if the provided <paramref name="node"/> is object and has a <c>Type</c> property, and if its value
    /// matches <typeparamref name="T"/>.
    /// </summary>
    public static bool HasMatchingTypeProperty<T>(this JsonNode node) =>
        node is JsonObject jsonObject && jsonObject["Type"]?.ToString() == typeof(T).Name;
}
