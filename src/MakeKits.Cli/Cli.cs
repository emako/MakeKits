using Newtonsoft.Json;
using System.Reflection;

namespace MakeKits.Cli;

internal static class Cli
{
    public static void Process(string[] args)
    {
        Console.WriteLine($"Startup: makekits v{Assembly.GetCallingAssembly().GetName().Version.ToString(3)}");

        if (args.Length <= 0)
        {
            if (!File.Exists("makekits.json"))
            {
                Console.WriteLine("Usage: makekits build \"path/to/makekits.json\"");

                // Notice: makekits eq. makekits init web
                args = ["init", "web"];
            }
            else
            {
                // Notice: makekits eq. makekits build makekits.json
                args = ["build", "makekits.json"];
            }
        }

        if (args.Length >= 1)
        {
            if (args[0] == "init")
            {
                Init(args);
            }
            else if (args[0] == "build")
            {
                // Executed by specified
                Build(args);
            }
        }

        // Executed by default
        Build(args);
    }

    public static void Init(string[] args)
    {
        if (args.Length == 2)
        {
            if (args[1] == "web" || args[1] == "webview")
            {
                InitWeb(args);
            }
            else if (args[1] == "exe" || args[1] == "exec" || args[1] == "executable")
            {
                InitExe(args);
            }
            else if (args[1] == "con" || args[1] == "console")
            {
                InitCon(args);
            }
        }
        else
        {
            InitWeb(args);
        }

        // Ensure that a command only does one thing
        Environment.Exit(0);

        static void InitWeb(string[] args)
        {
            _ = args;

            if (File.Exists("makekits.json"))
            {
                Console.WriteLine($@"ERR: File '{Environment.CurrentDirectory}\makekits.json' already exists, please delete it firstly.");
                Environment.Exit(-1);
            }
            else
            {
                Configuration config = new()
                {
                    MinimalVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3),
                    Template = "${KitsDir}/template/webview.7z",
                    Guid = Guid.NewGuid().ToString().Trim('{', '}'),
                    LaunchType = null!, // Out of topic
                    ExecName = null!, // Out of topic
                    ResizeOffsetWidth = 0, // Out of topic
                    ResizeOffsetHeight = 0, // Out of topic
                };

                File.WriteAllText("makekits.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                Console.WriteLine($@"INF: File '{Environment.CurrentDirectory}\makekits.json' created.");
            }
        }

        static void InitExe(string[] args)
        {
            _ = args;

            if (File.Exists("makekits.json"))
            {
                Console.WriteLine($@"ERR: File '{Environment.CurrentDirectory}\makekits.json' already exists, please delete it firstly.");
                Environment.Exit(-1);
            }
            else
            {
                Configuration config = new()
                {
                    MinimalVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3),
                    Template = "${KitsDir}/template/executable.7z",
                    Guid = Guid.NewGuid().ToString().Trim('{', '}'),
                    Resource = null!, // Out of topic
                    ResourceDirectory = null!, // Out of topic
                };

                File.WriteAllText("makekits.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                Console.WriteLine($@"INF: File '{Environment.CurrentDirectory}\makekits.json' had been created.");
            }
        }

        static void InitCon(string[] args)
        {
            _ = args;

            if (File.Exists("makekits.json"))
            {
                Console.WriteLine($@"ERR: File '{Environment.CurrentDirectory}\makekits.json' already exists, please delete it firstly.");
                Environment.Exit(-1);
            }
            else
            {
                Configuration config = new()
                {
                    MinimalVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3),
                    Template = "${KitsDir}/template/console.7z",
                    Guid = Guid.NewGuid().ToString().Trim('{', '}'),
                    LaunchType = "Console",
                    ExecName = "powershell.exe",
                    Resource = null!, // Out of topic
                    ResourceDirectory = null!, // Out of topic
                };

                File.WriteAllText("makekits.json", JsonConvert.SerializeObject(config, Formatting.Indented));
                Console.WriteLine($@"INF: File '{Environment.CurrentDirectory}\makekits.json' created.");
            }
        }
    }

    public static void Build(string[] args)
    {
        if (args.Length < 2)
        {
            Environment.Exit(-1);
        }

        Configuration config = null!;
        string path = args[1];

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
            Environment.Exit(-2);
        }

        if (!string.IsNullOrWhiteSpace(config.MinimalVersion))
        {
            Version toolVersion = Assembly.GetCallingAssembly().GetName().Version!;
            if (Version.TryParse(config.MinimalVersion, out Version? minVersion))
            {
                if (toolVersion < minVersion)
                {
                    Console.WriteLine($"ERR: makekits v{toolVersion.ToString(3)} is lower than the required minimal version v{minVersion.ToString(3)} specified in '{path}'.");
                    Environment.Exit(-4);
                }
            }
            else
            {
                Console.WriteLine($"ERR: Invalid MinimalVersion value '{config.MinimalVersion}' in '{path}'.");
                Environment.Exit(-5);
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
            Environment.Exit(-3);
        }

        // Ensure that a command only does one thing
        Environment.Exit(0);
    }
}
