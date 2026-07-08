using System.Xml.Linq;

namespace MakeKits.Cli.Core;

internal static class CSharpProject
{
    public static void SetupConfig(string csprojPath, Configuration config)
    {
        if (!File.Exists(csprojPath))
        {
            return;
        }

        XDocument doc = XDocument.Load(csprojPath);

        if (!string.IsNullOrWhiteSpace(config.TargetFramework))
        {
            doc.Element("Project")
               .Element("PropertyGroup")
               .SetElementValue("TargetFramework", config.TargetFramework);
        }

        if (!string.IsNullOrWhiteSpace(config.AssemblyName))
        {
            doc.Element("Project")
               .Element("PropertyGroup")
               .SetElementValue("AssemblyName", config.AssemblyName);
        }

        doc.Save(csprojPath);
    }
}
