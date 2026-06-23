using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MakeKits.Workshop.Webview.HelloWorld.Host;

public sealed class WorkshopManager
{
    private static WorkshopManager _instance = null!;

    public static string UserWorkshopDirectory => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"MakeKits.Workshop\");

    public static string BultinWorkshopDirectory => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "workshops");

    public static IReadOnlyList<IWorkshopItem> LoadedWorkshops { get; private set; } = [];

    private WorkshopManager()
    {
        LoadWorkshops(UserWorkshopDirectory);
        LoadWorkshops(BultinWorkshopDirectory);
        InitLoadedWorkshops();
    }

    public static WorkshopManager GetInstance()
    {
        return _instance ??= new WorkshopManager();
    }

    public static void LoadWorkshops(string folder)
    {
        if (!Directory.Exists(folder))
        {
            return;
        }

        List<IWorkshopItem> workshops = [];
        string[] libraries = [.. Directory.GetFiles(folder, "*Workshop.*.dll", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith("MakeKits.Workshop.Abstractions.dll")
                && !file.EndsWith("MakeKits.Workshop.Webview.dll")
                && !file.EndsWith("MakeKits.Workshop.WPF.dll"))];

        foreach (string library in libraries)
        {
            IWorkshopItem? workshop = CreateWorkshopInfo(library);

            if (workshop != null)
                workshops.Add(workshop);
        }

        LoadedWorkshops = [.. workshops
            .OrderBy(workshop => workshop.Name)
            .ThenBy(workshop => workshop.FileName)];
    }

    private static IWorkshopItem? CreateWorkshopInfo(string lib)
    {
        try
        {
            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(lib);
            string fileName = Path.GetFileName(lib);
            string name = versionInfo.ProductName;
            string version = versionInfo.ProductVersion;
            string author = versionInfo.CompanyName;

            AssemblyName assemblyName = AssemblyName.GetAssemblyName(lib);
            name = string.IsNullOrWhiteSpace(name) ? assemblyName.Name : name;
            version = string.IsNullOrWhiteSpace(version) ? assemblyName.Version?.ToString()! : version;

            IWorkshop workshop = Assembly.LoadFrom(lib)
                .GetExportedTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract
                    && typeof(IWorkshop).IsAssignableFrom(t))
                .Select(t => t.CreateInstance<IWorkshop>())
                .FirstOrDefault();

            return new WorkshopItem()
            {
                Name = string.IsNullOrWhiteSpace(name) ? Path.GetFileNameWithoutExtension(fileName) : name,
                Author = string.IsNullOrWhiteSpace(author) ? "-" : author,
                Version = string.IsNullOrWhiteSpace(version) ? "-" : version,
                FileName = fileName,
                FilePath = lib,
                Workshop = workshop,
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[WorkshopManager] Failed to read workshop assembly information: {lib}, {ex.Message}");
            return null;
        }
    }

    private void InitLoadedWorkshops()
    {
        for (int i = 0; i < LoadedWorkshops.Count; i++)
        {
            try
            {
                LoadedWorkshops[i].Workshop?.Init();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }
    }
}
