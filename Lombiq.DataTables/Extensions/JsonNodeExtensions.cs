namespace System.Text.Json.Nodes;

public static class JsonNodeExtensions
{
    /// <summary>
    /// Checks if the provided <paramref name="node"/> is object and has a <c>Type</c> property, and if its value
    /// matches <typeparamref name="T"/>.
    /// </summary>
    public static bool HasTypeProperty<T>(this JsonNode node) =>
        node?.AsObject()?["Type"]?.ToString() == typeof(T).Name;
}
