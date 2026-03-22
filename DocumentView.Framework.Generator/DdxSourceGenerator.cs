using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace DocumentView.Framework.Generator;

[Generator]
public sealed class DdxSourceGenerator : IIncrementalGenerator
{
    // Display format: qualified names + C# keywords for primitives (string, int, bool, ...)
    private static readonly SymbolDisplayFormat s_typeFormat = new SymbolDisplayFormat(
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions: SymbolDisplayMiscellaneousOptions.UseSpecialTypes
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var models = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (node, _) =>
                    node is ClassDeclarationSyntax cls &&
                    cls.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)),
                transform: static (ctx, ct) => GetClassModel(ctx, ct))
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        context.RegisterSourceOutput(models, static (spc, model) =>
        {
            var source = DdxEntryEmitter.Emit(model);
            spc.AddSource($"{model.ClassName}.DdxGenerated.g.cs",
                          SourceText.From(source, Encoding.UTF8));
        });
    }

    private static DdxClassModel? GetClassModel(GeneratorSyntaxContext ctx, CancellationToken ct)
    {
        var cls = (ClassDeclarationSyntax)ctx.Node;
        if (ctx.SemanticModel.GetDeclaredSymbol(cls, ct) is not INamedTypeSymbol symbol)
            return null;

        if (!InheritsFromMfcDocument(symbol))
            return null;

        var members = new List<DdxMemberModel>();

        foreach (var member in symbol.GetMembers())
        {
            ct.ThrowIfCancellationRequested();

            ITypeSymbol? memberType = null;
            bool canWrite = true;

            if (member is IFieldSymbol field)
            {
                memberType = field.Type;
            }
            else if (member is IPropertySymbol prop)
            {
                memberType = prop.Type;
                canWrite = !prop.IsReadOnly;
            }
            else
            {
                continue;
            }

            foreach (var attr in member.GetAttributes())
            {
                if (attr.AttributeClass?.Name != "DDXAttribute")
                    continue;
                if (attr.ConstructorArguments.Length == 0)
                    continue;

                var controlName = attr.ConstructorArguments[0].Value as string ?? string.Empty;
                string? controlProperty = null;
                foreach (var namedArg in attr.NamedArguments)
                {
                    if (namedArg.Key == "ControlProperty")
                    {
                        controlProperty = namedArg.Value.Value as string;
                        break;
                    }
                }

                var validators = new List<DdxValidatorModel>();
                foreach (var ddvAttr in member.GetAttributes())
                {
                    if (ddvAttr.AttributeClass is null)
                        continue;
                    if (!IsDdvAttribute(ddvAttr.AttributeClass))
                        continue;

                    var ctorArgs = new List<string>();
                    foreach (var arg in ddvAttr.ConstructorArguments)
                        ctorArgs.Add(FormatConstant(arg));

                    string? message = null;
                    foreach (var namedArg in ddvAttr.NamedArguments)
                    {
                        if (namedArg.Key == "Message")
                        {
                            message = namedArg.Value.Value as string;
                            break;
                        }
                    }

                    validators.Add(new DdxValidatorModel(
                        ddvAttr.AttributeClass.ToDisplayString(s_typeFormat),
                        ctorArgs,
                        message
                    ));
                }

                members.Add(new DdxMemberModel(
                    member.Name,
                    memberType.ToDisplayString(s_typeFormat),
                    canWrite,
                    controlName,
                    controlProperty,
                    validators
                ));
            }
        }

        if (members.Count == 0)
            return null;

        var namespaceName = symbol.ContainingNamespace.IsGlobalNamespace
            ? string.Empty
            : symbol.ContainingNamespace.ToDisplayString();

        return new DdxClassModel(namespaceName, symbol.Name, members);
    }

    private static bool InheritsFromMfcDocument(INamedTypeSymbol symbol)
    {
        var current = symbol.BaseType;
        while (current is not null)
        {
            if (current.Name == "MfcDocument" &&
                current.ContainingNamespace?.ToDisplayString() == "DocumentView.Framework")
                return true;
            current = current.BaseType;
        }
        return false;
    }

    private static bool IsDdvAttribute(INamedTypeSymbol symbol)
    {
        INamedTypeSymbol? current = symbol;
        while (current is not null)
        {
            if (current.Name == "DDVAttribute" &&
                current.ContainingNamespace?.ToDisplayString() == "DocumentView.Framework")
                return true;
            current = current.BaseType;
        }
        return false;
    }

    private static string FormatConstant(TypedConstant constant)
    {
        if (constant.Kind == TypedConstantKind.Array)
            return "null";

        var value = constant.Value;
        return value switch
        {
            null    => "null",
            bool b  => b ? "true" : "false",
            string s => "\"" + EscapeString(s) + "\"",
            double d => FormatDouble(d),
            float f  => FormatDouble(f),
            int i    => i.ToString(CultureInfo.InvariantCulture),
            long l   => l.ToString(CultureInfo.InvariantCulture) + "L",
            _        => value.ToString() ?? "null"
        };
    }

    private static string FormatDouble(double d)
    {
        if (double.IsNaN(d)) return "double.NaN";
        if (double.IsPositiveInfinity(d)) return "double.PositiveInfinity";
        if (double.IsNegativeInfinity(d)) return "double.NegativeInfinity";
        var s = d.ToString("G", CultureInfo.InvariantCulture);
        // Ensure the literal is parseable as double without a suffix
        if (!s.Contains(".") && !s.Contains("E") && !s.Contains("e"))
            s += ".0";
        return s;
    }

    private static string EscapeString(string s) =>
        s.Replace("\\", "\\\\")
         .Replace("\"", "\\\"")
         .Replace("\n", "\\n")
         .Replace("\r", "\\r");
}
