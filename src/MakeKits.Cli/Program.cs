using MakeKits.Cli;
using Newtonsoft.Json;
using System.Reflection;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine($"Startup: makekits v{Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");

        if (args.Length <= 0)
        {
            if (!File.Exists("makekits.json"))
            {
                Console.WriteLine("Usage: makekits \"path/to/makekits.json\"");
                Environment.ExitCode = -1;
                return;
            }

            args = ["makekits.json"];
        }

        if (args.Length == 1)
        {
            if (args[0] == "init")
            {
                if (File.Exists("makekits.json"))
                {
                    Console.WriteLine($"ERR: File '{Environment.CurrentDirectory}\\makekits.json' already exists.");
                    Environment.ExitCode = -1;
                    return;
                }
                else
                {
                    File.WriteAllText("makekits.json", JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));
                    Console.WriteLine($"INF: File '{Environment.CurrentDirectory}\\makekits.json' had been created.");
                    return;
                }
            }
        }

        Configuration config;
        string path = args[0];

        try
        {
            if (File.Exists(path))
            {
                string jsonString = File.ReadAllText(path);

                Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(path));
                Console.WriteLine($"INF: Change directory to '{Environment.CurrentDirectory}'.");

                if (!string.IsNullOrWhiteSpace(jsonString))
                {
                    config = JsonConvert.DeserializeObject<Configuration>(jsonString)
                        ?? throw new ArgumentException($"File '{path}' is empty.");
                }
                else
                {
                    throw new ArgumentException($"File '{path}' is empty.");
                }
            }
            else
            {
                throw new ArgumentException($"File '{path}' not found.");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("ERR: " + e.Message);
            Environment.ExitCode = -2;
            return;
        }

        if (!string.IsNullOrWhiteSpace(config.MinimalVersion))
        {
            Version toolVersion = Assembly.GetCallingAssembly().GetName().Version!;
            if (Version.TryParse(config.MinimalVersion, out Version? minVersion))
            {
                if (toolVersion < minVersion)
                {
                    Console.WriteLine($"ERR: makekits v{toolVersion.ToString(3)} is lower than the required minimal version v{minVersion.ToString(3)} specified in '{path}'.");
                    Environment.ExitCode = -4;
                    return;
                }
            }
            else
            {
                Console.WriteLine($"ERR: Invalid MinimalVersion value '{config.MinimalVersion}' in '{path}'.");
                Environment.ExitCode = -5;
                return;
            }
        }

        try
        {
            App app = new();
            app.Run(config);
        }
        catch (Exception e)
        {
            Console.WriteLine("ERR: " + e.Message);
            Environment.ExitCode = -3;
        }
    }
}
