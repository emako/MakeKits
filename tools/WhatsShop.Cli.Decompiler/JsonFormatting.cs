using System.Text.Json;
using System.Text.Json.Serialization;

namespace WhatsShop;

internal static class JsonFormatting
{
    public static string SerializeFormatted(WorkshopMetaInfo value)
        => JsonSerializer.Serialize(value, WorkshopJsonContext.Default.WorkshopMetaInfo);
}

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(WorkshopMetaInfo))]
internal partial class WorkshopJsonContext : JsonSerializerContext;
