using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics;

namespace MakeKits.Cli.Extension;

internal static class SyntaxNodeExtensions
{
    public static CompilationUnitSyntax ReplaceAssemblyAttributeWithString(this CompilationUnitSyntax root, string name, string value)
    {
        var attributeList = root.AttributeLists
          .SelectMany(al => al.Attributes)
          .FirstOrDefault(attr => attr.Name.ToString() == name);

        if (attributeList != null)
        {
            var newArgument = SyntaxFactory.AttributeArgument(
            SyntaxFactory.LiteralExpression(
                SyntaxKind.StringLiteralExpression,
                SyntaxFactory.Literal(value)));

            var newArgumentList = SyntaxFactory.AttributeArgumentList(
                SyntaxFactory.SeparatedList([newArgument]));

            var newAttributeNode = attributeList.WithArgumentList(newArgumentList);

            return root.ReplaceNode(attributeList, newAttributeNode);
        }

        return root;
    }

    public static CompilationUnitSyntax ReplaceOptionWithString(this CompilationUnitSyntax root, string optionName, string? value)
    {
        if (value == null)
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
        else
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression, SyntaxFactory.Literal(value));
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
    }

    public static CompilationUnitSyntax ReplaceOptionWithInt32(this CompilationUnitSyntax root, string optionName, int? value)
    {
        if (value == null)
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
        else
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value.Value));
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
    }

    public static CompilationUnitSyntax ReplaceOptionWithDouble(this CompilationUnitSyntax root, string optionName, double? value)
    {
        if (value == null)
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression);
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
        else
        {
            var newRight = SyntaxFactory.LiteralExpression(SyntaxKind.NumericLiteralExpression, SyntaxFactory.Literal(value.Value));
            return root.ReplaceOptionWithAny(optionName, newRight);
        }
    }

    public static CompilationUnitSyntax ReplaceOptionWithAny(this CompilationUnitSyntax root, string optionName, ExpressionSyntax newRight)
    {
        var propertyNode = root.DescendantNodes()
            .OfType<PropertyDeclarationSyntax>()
            .FirstOrDefault(p => p.Identifier.Text == optionName);

        if (propertyNode != null)
        {
            var newInitializer = propertyNode.Initializer != null
                ? propertyNode.Initializer.WithValue(newRight)
                : SyntaxFactory.EqualsValueClause(newRight);

            var newProperty = propertyNode.WithInitializer(newInitializer);
            return root.ReplaceNode(propertyNode, newProperty);
        }

        Console.WriteLine($"[ERR] `{optionName}` property not found.");
        return root;
    }

    [Conditional("DEBUG")]
    public static void PrintSyntaxTree(this SyntaxNode node, int indentLevel = 0)
    {
        var indent = new string(' ', indentLevel * 2);
        Console.WriteLine($"{indent}{node.Kind()}: {node}");

        foreach (var child in node.ChildNodesAndTokens())
        {
            if (child.IsNode)
            {
                PrintSyntaxTree(child.AsNode()!, indentLevel + 1);
            }
        }
    }
}
