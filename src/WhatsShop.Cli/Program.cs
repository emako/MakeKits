using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WhatsShop;

internal static class Program
{
    private static readonly JsonSerializerSettings JsonSettings = new()
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver(),
        Formatting = Formatting.Indented,
        NullValueHandling = NullValueHandling.Include,
    };

    static int Main(string[] args)
    {
        if (args.Length != 1 || args[0] is "-h" or "--help" or "/?" or "-?")
        {
            Console.Error.WriteLine("Usage: whatsshop path/to/file.dll");
            return args.Length == 1 ? 0 : 1;
        }

        string path = args[0];
        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"File not found: {path}");
            return 1;
        }

        if (WorkshopMetadataReader.TryRead(path) is WorkshopMetaInfo meta)
        {
            Console.WriteLine(JsonConvert.SerializeObject(meta, JsonSettings));
            return 0;
        }
        else
        {
            Console.WriteLine("{}");
            return 1;
        }
    }
}
