namespace WhatsShop;

internal static class Program
{
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
            Console.WriteLine(JsonFormatting.SerializeFormatted(meta));
            return 0;
        }
        else
        {
            Console.WriteLine("{}");
            return 1;
        }
    }
}
