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

        Makeup config;
        string path = args[0];
    }
}
