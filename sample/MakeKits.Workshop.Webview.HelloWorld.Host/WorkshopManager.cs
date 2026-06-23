using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace MakeKits.Workshop.Webview.HelloWorld.Host;

public static class WorkshopManager
{
    private static readonly Regex WorkshopFileRegex = new(@"^Workshop\..*\.dll$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public static string WorkshopDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workshops");

    public static IReadOnlyList<IWorkshopItem> LoadWorkshops()
    {
        if (!Directory.Exists(WorkshopDirectory))
        {
            return [];
        }

        List<IWorkshopItem> workshops = [];
        string[] libraries = [.. Directory.GetFiles(WorkshopDirectory, "*Workshop.*.dll", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith("MakeKits.Workshop.Abstractions.dll") && !file.EndsWith("MakeKits.Workshop.Webview.dll"))];

        foreach (string library in libraries)
        {
            IWorkshopItem workshop = CreateWorkshopInfo(library);
            workshops.Add(workshop);
        }

        return [.. workshops
            .OrderBy(workshop => workshop.Name)
            .ThenBy(workshop => workshop.FileName)];
    }

    private static IWorkshopItem CreateWorkshopInfo(string library)
    {
        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(library);
        string fileName = Path.GetFileName(library);
        string name = versionInfo.ProductName;
        string version = versionInfo.ProductVersion;
        string author = versionInfo.CompanyName;

        try
        {
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(library);
            name = string.IsNullOrWhiteSpace(name) ? assemblyName.Name : name;
            version = string.IsNullOrWhiteSpace(version) ? assemblyName.Version?.ToString()! : version;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[WorkshopManager] Failed to read workshop assembly information: {library}, {ex.Message}");
        }

        return new WorkshopItem()
        {
            Name = string.IsNullOrWhiteSpace(name) ? Path.GetFileNameWithoutExtension(fileName) : name,
            Author = string.IsNullOrWhiteSpace(author) ? "-" : author,
            Version = string.IsNullOrWhiteSpace(version) ? "-" : version,
            FileName = fileName,
            FilePath = library,
        };
    }
}
