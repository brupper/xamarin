using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using OData.Generators.Templates;

namespace OData.Generators;

[
    Generator(LanguageNames.CSharp)
]
public sealed class ServiceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        Debug.WriteLine("Initialize code generator");

        var attributedClasses = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(GenerateServiceAttribute).FullName!,
            static (node, _) => node is ClassDeclarationSyntax,
            static (syntaxContext, _) =>
            {
                var classSymbol = (INamedTypeSymbol)syntaxContext.TargetSymbol;
                var attributeData = syntaxContext.Attributes[0];
                return new ServiceGenerationTarget(classSymbol, attributeData);
            });

        var additionalFiles = context.AdditionalTextsProvider.Collect();
        var combined = attributedClasses.Collect().Combine(additionalFiles);

        context.RegisterSourceOutput(combined, (sourceProductionContext, source) =>
        {
            var (targets, files) = source;
            if (targets.IsDefaultOrEmpty)
            {
                return;
            }

            foreach (var target in targets)
            {
                var templateParameter = GetConstructorArgument(target.AttributeData, 0);
                var className = GetClassName(target.AttributeData);
                var overriddenTemplate = TryGetTemplate(files, templateParameter);
                var sourceCode = GetSourceCodeFor(target.Symbol, overriddenTemplate, className);

                sourceProductionContext.AddSource(
                    $"{target.Symbol.Name}{templateParameter ?? "Controller"}.g.cs",
                    SourceText.From(sourceCode, Encoding.UTF8));
            }
        });
    }

    private static string GetSourceCodeFor(INamedTypeSymbol symbol, string? template = null, string? className = null)
    {
        // If template isn't provided, use default one from embeded resources.
        template ??= GetEmbeddedResource("Brupper.CodeGenerators.OData.Templates.Default.txt");

        // Can't use scriban at the moment, make it manually for now.
        return template
            .Replace("{{" + nameof(DefaultTemplateParameters.ClassName) + "}}", className ?? symbol.Name)
            .Replace("{{" + nameof(DefaultTemplateParameters.Namespace) + "}}", GetNamespaceRecursively(symbol.ContainingNamespace))
            .Replace("{{" + nameof(DefaultTemplateParameters.PrefferredNamespace) + "}}", symbol.ContainingAssembly.Name)
            ;
    }

    private static string GetEmbeddedResource(string path)
    {
        using var stream = typeof(ServiceGenerator).Assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }

    private static string GetNamespaceRecursively(INamespaceSymbol symbol)
    {
        if (symbol.ContainingNamespace == null)
        {
            return symbol.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }

    private static string? GetConstructorArgument(AttributeData attributeData, int index)
    {
        if (attributeData.ConstructorArguments.Length <= index)
        {
            return null;
        }

        return attributeData.ConstructorArguments[index].Value as string;
    }

    private static string? GetClassName(AttributeData attributeData)
    {
        var className = GetConstructorArgument(attributeData, 1);
        if (!string.IsNullOrWhiteSpace(className))
        {
            return className;
        }

        foreach (var namedArgument in attributeData.NamedArguments)
        {
            if (namedArgument.Key == nameof(GenerateServiceAttribute.ClassName))
            {
                return namedArgument.Value.Value as string;
            }
        }

        return null;
    }

    private static string? TryGetTemplate(ImmutableArray<AdditionalText> additionalFiles, string? templateParameter)
    {
        if (string.IsNullOrWhiteSpace(templateParameter))
        {
            return null;
        }

        var file = additionalFiles.FirstOrDefault(x => x.Path.EndsWith(templateParameter, StringComparison.OrdinalIgnoreCase));
        return file?.GetText()?.ToString();
    }

    private sealed class ServiceGenerationTarget
    {
        public ServiceGenerationTarget(INamedTypeSymbol symbol, AttributeData attributeData)
        {
            Symbol = symbol;
            AttributeData = attributeData;
        }

        public INamedTypeSymbol Symbol { get; }

        public AttributeData AttributeData { get; }
    }
}
