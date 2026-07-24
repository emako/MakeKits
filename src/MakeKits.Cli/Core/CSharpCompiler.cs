using Flucli;
using System.Runtime.InteropServices;
using System.Text;

namespace MakeKits.Cli.Core;

internal static class CSharpCompiler
{
    public static void Build(Configuration config)
    {
        _ = config ?? throw new ArgumentNullException(nameof(config));

        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException("CSharp compilation is only supported on Windows.");
        }

        CSharpScript.EnsureBuildEnvironment();

        if (!CSharpScript.IsBuildEnvironmentReady(out _, out string msbuild))
        {
            throw new ApplicationException("Build environment is not ready after setup.");
        }

        string csproj = Directory.EnumerateFiles(".dist", "*.csproj", SearchOption.TopDirectoryOnly)
            .FirstOrDefault()
            ?? throw new FileNotFoundException("No .csproj found in .dist");

        Console.OutputEncoding = Encoding.UTF8;

        string arguments =
            $@"""{csproj}"" /t:Rebuild /p:Configuration=Release /p:ImportDirectoryBuildProps=false /p:RestoreUseStaticGraphEvaluation=false /restore /p:RestoreSources=""https://api.nuget.org/v3/index.json""";

        CliResult buildResult = msbuild
            .WithArguments(arguments)
            .WithStandardOutputPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Out.WriteLine(line);
            }))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Error.WriteLine(line);
            }))
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        if (!buildResult.IsSuccess)
        {
            throw new Exception("Build failed.");
        }

        string builtDll = Path.Combine(".dist", "bin", "Release", config.AssemblyName + ".dll");

        if (!File.Exists(builtDll))
        {
            throw new FileNotFoundException($"Built assembly not found: {builtDll}");
        }

        string output = config.Output;
        string? outputDir = Path.GetDirectoryName(Path.GetFullPath(output));

        if (!string.IsNullOrEmpty(outputDir))
        {
            _ = Directory.CreateDirectory(outputDir);
        }

        File.Copy(builtDll, output, true);
        Console.WriteLine("Output: " + output);
    }
}
