using System.Text;

namespace WhatsShop;

internal static class JsonFormatting
{
    public static string SerializeFormatted(object value) => PrettyPrint(value.ToJson());

    private static string PrettyPrint(string json)
    {
        if (string.IsNullOrEmpty(json))
        {
            return json;
        }

        StringBuilder result = new();
        int depth = 0;
        bool inString = false;

        for (int i = 0; i < json.Length; i++)
        {
            char c = json[i];
            if (c == '"')
            {
                int backslashes = 0;
                for (int j = i - 1; j >= 0 && json[j] == '\\'; j--)
                {
                    backslashes++;
                }

                if (backslashes % 2 == 0)
                {
                    inString = !inString;
                }
            }

            if (!inString)
            {
                switch (c)
                {
                    case '{':
                    case '[':
                        result.Append(c);
                        result.AppendLine();
                        depth++;
                        result.Append(new string(' ', depth * 2));
                        continue;
                    case '}':
                    case ']':
                        result.AppendLine();
                        depth--;
                        result.Append(new string(' ', depth * 2));
                        result.Append(c);
                        continue;
                    case ',':
                        result.Append(c);
                        result.AppendLine();
                        result.Append(new string(' ', depth * 2));
                        continue;
                    case ':':
                        result.Append(": ");
                        continue;
                    case ' ':
                        continue;
                }
            }

            result.Append(c);
        }

        return result.ToString();
    }
}
