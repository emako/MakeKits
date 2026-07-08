using MakeKits.Cli.Extension;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        if (!string.IsNullOrWhiteSpace(config.Name))
        {
            root = root.ReplaceOptionWithString("Name", config.Name);
        }

        File.Delete(csPath);
        File.WriteAllText(csPath, root.ToString());
    }
}
