using MakeKits.Cli.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace MakeKits.Cli.Core;

internal static class CSharpConfig
{
    public static void SetupConfig(string csPath, Configuration config)
    {
        if (!File.Exists(csPath))
        {
            return;
        }

        string code = File.ReadAllText(csPath);
        SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
        CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

        if (!string.IsNullOrWhiteSpace(config.Guid))
        {
            if (Guid.TryParse(config.Guid, out _))
            {
                root = root.ReplaceAssemblyAttributeWithString("Guid", config.Guid);
            }
            else
            {
                throw new ArgumentException($"Invalid `Guid` of '{config.Guid}'.");
            }
        }

        if (!string.IsNullOrWhiteSpace(config.Id))
        {
            root = root.ReplaceAssemblyAttributeWithString("Workshop", config.Id);
            root = root.ReplaceOptionWithString("Id", config.Id);
        }

        if (!string.IsNullOrWhiteSpace(config.Name))
        {
            root = root.ReplaceOptionWithString("Name", config.Name);
        }

        if (!string.IsNullOrWhiteSpace(config.Author))
        {
            root = root.ReplaceOptionWithString("Author", config.Author);
        }

        if (!string.IsNullOrWhiteSpace(config.Description))
        {
            root = root.ReplaceOptionWithString("Description", config.Description);
        }

        if (!string.IsNullOrWhiteSpace(config.Title))
        {
            root = root.ReplaceOptionWithString("Title", config.Title);
        }

        if (!string.IsNullOrWhiteSpace(config.Theme))
        {
            root = root.ReplaceOptionWithString("Theme", config.Theme);
        }

        if (!string.IsNullOrWhiteSpace(config.UserDataFolder))
        {
            root = root.ReplaceOptionWithString("UserDataFolder", config.UserDataFolder);
        }

        if (!string.IsNullOrWhiteSpace(config.LaunchType))
        {
            root = root.ReplaceOptionWithString("LaunchType", config.LaunchType);
        }

        if (!string.IsNullOrWhiteSpace(config.ExecName))
        {
            root = root.ReplaceOptionWithString("ExecName", config.ExecName);
        }

        root = root.ReplaceOptionWithInt32("ResizeOffsetWidth", config.ResizeOffsetWidth);
        root = root.ReplaceOptionWithInt32("ResizeOffsetHeight", config.ResizeOffsetHeight);

        File.Delete(csPath);
        File.WriteAllText(csPath, root.ToString());
    }
}
