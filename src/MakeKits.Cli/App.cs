using MakeKits.Cli.Core;
using MakeKits.Cli.Helper;
using PureSharpCompress.Common;

namespace MakeKits.Cli;

internal sealed class App
{
    public void Run(Configuration config = null!)
    {
        _ = config ?? throw new ArgumentNullException(nameof(config));

        // Solve Marco, CAN'T change the order.
        {
            config.Template = config.Template.SolveTemplate();
            config.AssemblyName = config.AssemblyName.SolveAssemblyName(config);
            config.Output = config.Output.SolveOutput(config);
        }

        // Extract template files.
        {
            string? template = Macro.GetFullPath(config.Template);
            string? package = Macro.GetFullPath(config.Package);
            string? resource = Macro.GetFullPath(config.Resource);

            if (!File.Exists(template))
            {
                Console.WriteLine($"ERR: Template file '{template}' not found.");
                return;
            }

            if (!string.IsNullOrWhiteSpace(resource))
            {
                if (!File.Exists(resource))
                {
                    Console.WriteLine($"ERR: Resource file '{resource}' not found.");
                    return;
                }
            }

            if (Directory.Exists(".dist"))
            {
                Directory.Delete(".dist", true);
            }
            _ = Directory.CreateDirectory(".dist");

            ArchiveFileHelper.ExtractAll(".dist", template, options: new ExtractionOptions()
            {
                ExtractFullPath = true,
                Overwrite = true,
                PreserveAttributes = false,
                PreserveFileTime = true,
            });
        }

        // Apply your config
        {
            foreach (string file in Directory.EnumerateFiles(".dist", "*.csproj", SearchOption.TopDirectoryOnly))
            {
                CSharpProject.SetupConfig(file, config);
            }
            CSharpConfig.SetupConfig(@".dist\Configuration.cs", config);
        }
    }
}
