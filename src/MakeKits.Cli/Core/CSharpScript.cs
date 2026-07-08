using Flucli;
using Flucli.Utils.Extensions;
using Microsoft.Win32;
using System.Net;
using System.Text;

namespace MakeKits.Cli.Core;

internal static class CSharpScript
{
    private const string BuildToolsUrl = "https://aka.ms/vs/stable/vs_BuildTools.exe";
    private const string BuildToolsProduct = "Microsoft.VisualStudio.Product.BuildTools";
    private const string Net48RefAssemblies = @"Reference Assemblies\Microsoft\Framework\.NETFramework\v4.8";

    public static bool TryFindVSWhere(out string path)
    {
        string? uninstallInfo = GetUninstallInfo(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", "Microsoft Visual Studio Installer")
                             ?? GetUninstallInfo(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", "Microsoft Visual Studio Installer");

        if (string.IsNullOrEmpty(uninstallInfo))
        {
            path = string.Empty;
            return false;
        }

        string[] parsedArgs = [.. uninstallInfo?.ToArguments() ?? []];

        if (parsedArgs.Length <= 0)
        {
            path = string.Empty;
            return false;
        }

        FileInfo uninst = new(parsedArgs[0].Trim('"'));
        string vswhere = Path.Combine(uninst.DirectoryName, "vswhere.exe");

        if (!File.Exists(vswhere))
        {
            path = string.Empty;
            return false;
        }

        path = vswhere;
        return true;
    }

    public static string FindVSWhere()
    {
        if (TryFindVSWhere(out string vswhere))
        {
            return vswhere;
        }

        throw new ApplicationException("Microsoft Visual Studio Installer is not installed, register not found.");
    }

    private static string? GetUninstallInfo(string keyPath, string displayName)
    {
        using RegistryKey key = Registry.LocalMachine.OpenSubKey(keyPath);

        if (key != null)
        {
            foreach (string subkeyName in key.GetSubKeyNames())
            {
                using RegistryKey subkey = key.OpenSubKey(subkeyName);

                if (subkey != null)
                {
                    if (subkey.GetValue("DisplayName") is string name && name.Contains(displayName))
                    {
                        string? uninstallString = subkey.GetValue("UninstallString") as string;

                        if (!string.IsNullOrEmpty(uninstallString))
                        {
                            return uninstallString;
                        }
                    }
                }
            }
        }
        return null;
    }

    public static bool IsNet48TargetingPackInstalled()
    {
        string refDir = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86),
            Net48RefAssemblies);

        return Directory.Exists(refDir)
            && File.Exists(Path.Combine(refDir, "mscorlib.dll"));
    }

    public static bool TryFindBuildToolsMsBuild(string vswhere, out string msbuild)
    {
        msbuild = string.Empty;

        if (string.IsNullOrWhiteSpace(vswhere) || !File.Exists(vswhere))
        {
            return false;
        }

        StringBuilder stdout = new();

        vswhere
            .WithArguments($"-products {BuildToolsProduct} -requires Microsoft.Component.MSBuild Microsoft.Net.Component.4.8.TargetingPack -find MSBuild\\**\\Bin\\MSBuild.exe")
            .WithStandardOutputPipe(PipeTarget.ToStringBuilder(stdout, Encoding.UTF8))
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        string[] lines = stdout.ToString()
            .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

        foreach (string line in lines)
        {
            string candidate = line.Trim();

            if (!string.IsNullOrEmpty(candidate) && File.Exists(candidate))
            {
                msbuild = candidate;
                return true;
            }
        }

        return false;
    }

    public static bool IsBuildEnvironmentReady(out string vswhere, out string msbuild)
    {
        vswhere = string.Empty;
        msbuild = string.Empty;

        if (!IsNet48TargetingPackInstalled())
        {
            return false;
        }

        if (!TryFindVSWhere(out vswhere))
        {
            return false;
        }

        return TryFindBuildToolsMsBuild(vswhere, out msbuild);
    }

    public static void EnsureBuildEnvironment()
    {
        if (IsBuildEnvironmentReady(out _, out _))
        {
            return;
        }

        LogMissingComponents();

        string installerPath = Path.Combine(Path.GetTempPath(), "vs_BuildTools.exe");

        Console.WriteLine($"INF: Downloading vs_BuildTools.exe from {BuildToolsUrl} ...");
        using (WebClient client = new())
        {
            client.DownloadFile(BuildToolsUrl, installerPath);
        }

        Console.WriteLine("INF: Installing Visual Studio Build Tools (ManagedDesktopBuildTools). This may take a while ...");
        Console.WriteLine("INF: Administrator privileges may be required.");

        CliResult installResult = installerPath
            .WithArguments("--passive --wait --norestart --add Microsoft.VisualStudio.Workload.ManagedDesktopBuildTools --includeRecommended --includeOptional")
            .WithStandardOutputPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Out.WriteLine(line);
            }))
            .WithStandardErrorPipe(PipeTarget.ToDelegate(static async (line, token) =>
            {
                Console.Error.WriteLine(line);
            }))
            .WithVerb("runas") // Request elevation (administrator privileges)
            .ExecuteAsync()
            .GetAwaiter()
            .GetResult();

        try
        {
            if (File.Exists(installerPath))
            {
                File.Delete(installerPath);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }

        if (!installResult.IsSuccess)
        {
            throw new ApplicationException(
                $"vs_BuildTools.exe installation failed with exit code {installResult.ExitCode}. " +
                "Try running makekits as administrator or install Build Tools manually.");
        }

        if (!IsBuildEnvironmentReady(out _, out _))
        {
            throw new ApplicationException(
                "Build environment is still incomplete after installation. " +
                DescribeMissingComponents() +
                " Try running makekits as administrator or install Build Tools manually.");
        }
    }

    private static void LogMissingComponents()
    {
        Console.WriteLine("INF: Build environment is not ready:");

        if (!IsNet48TargetingPackInstalled())
        {
            Console.WriteLine("INF:   - .NET Framework 4.8 targeting pack is missing.");
        }

        if (!TryFindVSWhere(out string vswhere))
        {
            Console.WriteLine("INF:   - vswhere.exe (Visual Studio Installer) is missing.");
        }
        else if (!TryFindBuildToolsMsBuild(vswhere, out _))
        {
            Console.WriteLine("INF:   - MSBuild from Visual Studio Build Tools is missing.");
        }
    }

    private static string DescribeMissingComponents()
    {
        List<string> missing = [];

        if (!IsNet48TargetingPackInstalled())
        {
            missing.Add(".NET Framework 4.8 targeting pack");
        }

        if (!TryFindVSWhere(out string vswhere))
        {
            missing.Add("vswhere.exe (Visual Studio Installer)");
        }
        else if (!TryFindBuildToolsMsBuild(vswhere, out _))
        {
            missing.Add("MSBuild from Visual Studio Build Tools");
        }

        return missing.Count > 0
            ? $" Missing: {string.Join(", ", missing)}."
            : string.Empty;
    }
}
