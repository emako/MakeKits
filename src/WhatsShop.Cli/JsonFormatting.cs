using System.Text;
using EleCho.Json;

namespace WhatsShop;

internal static class JsonFormatting
{
    public static string SerializeFormatted(object value)
    {
        IJsonData json = JsonData.FromValue(value);
        StringBuilder builder = new();
        Append(builder, json, 0);
        return builder.ToString();
    }

    private static void Append(StringBuilder builder, IJsonData data, int depth)
    {
        switch (data.DataKind)
        {
            case JsonDataKind.Object:
                AppendObject(builder, (JsonObject)data, depth);
                break;
            case JsonDataKind.Array:
                AppendArray(builder, (JsonArray)data, depth);
                break;
            default:
                builder.Append(WriteToken(data));
                break;
        }
    }

    private static void AppendObject(StringBuilder builder, JsonObject obj, int depth)
    {
        builder.Append('{');
        bool first = true;
        foreach (KeyValuePair<string, IJsonData> entry in obj)
        {
            if (!first)
            {
                builder.Append(',');
            }

            first = false;
            builder.AppendLine();
            builder.Append(Indent(depth + 1));
            builder.Append(WriteToken(new JsonString(ToCamelCase(entry.Key))));
            builder.Append(": ");
            Append(builder, entry.Value, depth + 1);
        }

        if (!first)
        {
            builder.AppendLine();
            builder.Append(Indent(depth));
        }

        builder.Append('}');
    }

    private static void AppendArray(StringBuilder builder, JsonArray array, int depth)
    {
        builder.Append('[');
        bool first = true;
        foreach (IJsonData item in array)
        {
            if (!first)
            {
                builder.Append(',');
            }

            first = false;
            builder.AppendLine();
            builder.Append(Indent(depth + 1));
            Append(builder, item, depth + 1);
        }

        if (!first)
        {
            builder.AppendLine();
            builder.Append(Indent(depth));
        }

        builder.Append(']');
    }

    private static string WriteToken(IJsonData data)
    {
        using StringWriter writer = new();
        new JsonWriter(writer).Write(data);
        return writer.ToString();
    }

    private static string Indent(int depth) => new(' ', depth * 2);

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name) || char.IsLower(name[0]))
        {
            return name;
        }

        if (name.Length == 1)
        {
            return name.ToLowerInvariant();
        }

        return char.ToLowerInvariant(name[0]) + name.Substring(1);
    }
}
