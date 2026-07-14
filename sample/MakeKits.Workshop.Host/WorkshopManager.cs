using MakeKits.Workshop.Webview;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MakeKits.Workshop.Host;

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

        List<IWorkshopItem> workshops = [.. LoadedWorkshops];
        string[] libraries = [.. Directory.GetFiles(folder, "*Workshop.*.dll", SearchOption.AllDirectories)
            .Where(file => !file.EndsWith("MakeKits.Workshop.Abstractions.dll")
                && !file.EndsWith("MakeKits.Workshop.Webview.dll")
                && !file.EndsWith("MakeKits.Workshop.WPF.dll"))];

        foreach (string library in libraries)
        {
            // Skip already-loaded paths to avoid duplicates.
            if (workshops.Any(w => string.Equals(w.FilePath, library, StringComparison.OrdinalIgnoreCase)))
                continue;

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
            AssemblyName assemblyName = AssemblyName.GetAssemblyName(lib);
            IWorkshop workshop = Assembly.LoadFrom(lib)
                .GetExportedTypes()
                .Where(t => !t.IsInterface && !t.IsAbstract
                    && typeof(IWorkshop).IsAssignableFrom(t))
                .Select(t => t.CreateInstance<IWorkshop>())
                .FirstOrDefault();

            string fileName = Path.GetFileName(lib);
            string version = $"v{assemblyName.Version}";
            string name = workshop?.Descriptor?.Name!;
            string author = workshop?.Descriptor?.Author!;
            string description = workshop?.Descriptor?.Description!;

            return new WorkshopItem()
            {
                Name = name,
                Author = author,
                Description = description,
                Version = version,
                FileName = fileName,
                FilePath = lib,
                Workshop = workshop!,
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
