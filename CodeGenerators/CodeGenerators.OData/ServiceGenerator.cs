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

// using System.Collections.Immutable;
//using System.Diagnostics;
//using System.Text;
//using Microsoft.CodeAnalysis;
//using Microsoft.CodeAnalysis.CSharp;
//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using Microsoft.CodeAnalysis.Text;
//using OData.Generators.Templates;

//namespace OData.Generators;

//[Generator(LanguageNames.CSharp)]
//public class ServiceGenerator : IIncrementalGenerator
//{
//    public void Initialize(IncrementalGeneratorInitializationContext context)
//    {
//        //#if DEBUG
//        //        if (!Debugger.IsAttached){Debugger.Launch();}
//        //#endif
//        Debug.WriteLine("Initialize code generator");

//        // context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateServiceAttribute>());

//        var syntaxProvider = context.SyntaxProvider
//            .CreateSyntaxProvider(
//                predicate: static (s, _) => s is ClassDeclarationSyntax,
//                transform: static (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
//            .Where(static m => m != null);

//        var compilationAndClasses = context.CompilationProvider.Combine(syntaxProvider.Collect());

//        context.RegisterSourceOutput(compilationAndClasses, (spc, source) => Execute(source.Left, source.Right, spc));
//    }

//    private void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
//    {
//        Debug.WriteLine("Execute code generator");

//        var sb = new StringBuilder();
//        sb.AppendLine("/*");
//        sb.AppendLine($"{DateTime.Now.ToShortTimeString()}");

//        var syntaxReceiver = new AttributeSyntaxReceiver<GenerateServiceAttribute>();

//        foreach (var classSyntax in classes)
//        {
//            syntaxReceiver.OnVisitSyntaxNode(classSyntax);
//        }

//        sb.AppendLine(string.Join("\r\n", syntaxReceiver.Log));
//        sb.AppendLine($"loop START, Any: {syntaxReceiver.Classes?.Any()}");
//        foreach (var classSyntax in syntaxReceiver.Classes)
//        {
//            sb.AppendLine("foreach");

//            var model = compilation.GetSemanticModel(classSyntax.SyntaxTree);
//            var symbol = model.GetDeclaredSymbol(classSyntax);

//            // Finding my GenerateServiceAttribute over it. I'm sure this attribute is placed, because my syntax receiver already checked before.
//            // So, I can surely execute following query.
//            var attribute = syntaxReceiver.GetFrom(classSyntax);

//            // Getting constructor parameter of the attribute. It might be not presented.
//            var templateParameter = attribute.ArgumentList?.Arguments.FirstOrDefault()?.GetLastToken().ValueText; // Temprorary... Attribute has only one argument for now.
//            var className = attribute.ArgumentList?.Arguments.Count > 1 ?
//                    attribute.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.GetLastToken().ValueText : null;

//            // Can't access embeded resource of main project.
//            // So overridden template must be marked as Analyzer Additional File to be able to be accessed by an analyzer.
//            var overridenTemplate = templateParameter != null ?
//                context.AdditionalFiles.FirstOrDefault(x => x.Path.EndsWith(templateParameter))?.GetText().ToString() :
//                null;

//            // Generate the real source code. Pass the template parameter if there is a overriden template.
//            var sourceCode = GetSourceCodeFor(symbol, overridenTemplate, className);

//            context.AddSource(
//                $"{symbol.Name}{templateParameter ?? "Controller"}.g.cs",
//                SourceText.From(sourceCode, Encoding.UTF8));

//            Debug.WriteLine(classSyntax);
//        }
//        sb.AppendLine("loop END");

//        sb.AppendLine("*/");
//        context.AddSource($"Log", SourceText.From(sb.ToString(), Encoding.UTF8));
//    }

//    private string GetSourceCodeFor(INamedTypeSymbol symbol, string template = null, string className = null)
//{
//    // If template isn't provided, use default one from embeded resources.
//    template ??= GetEmbeddedResource("Brupper.CodeGenerators.OData.Templates.Default.txt");

//    // Can't use scriban at the moment, make it manually for now.
//    return template
//        .Replace("{{" + nameof(DefaultTemplateParameters.ClassName) + "}}", className ?? symbol.Name)
//        .Replace("{{" + nameof(DefaultTemplateParameters.Namespace) + "}}", GetNamespaceRecursively(symbol.ContainingNamespace))
//        .Replace("{{" + nameof(DefaultTemplateParameters.PrefferredNamespace) + "}}", symbol.ContainingAssembly.Name)
//        ;
//}

//private string GetEmbeddedResource(string path)
//{
//    using var stream = GetType().Assembly.GetManifestResourceStream(path);

//    using var streamReader = new StreamReader(stream);

//    return streamReader.ReadToEnd();
//}

//private string GetNamespaceRecursively(INamespaceSymbol symbol)
//{
//    if (symbol.ContainingNamespace == null)
//    {
//        return symbol.Name;
//    }

//    return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
//}

//private static IEnumerable<ITypeSymbol> GetAllTypes(INamespaceSymbol root)
//{
//    foreach (var namespaceOrTypeSymbol in root.GetMembers())
//    {
//        if (namespaceOrTypeSymbol is INamespaceSymbol @namespace)
//        {
//            foreach (var nested in GetAllTypes(@namespace))
//            {
//                yield return nested;
//            }
//        }
//        else if (namespaceOrTypeSymbol is ITypeSymbol type)
//        {
//            yield return type;
//        }
//    }
//}

//static bool IsSyntaxTargetForGeneration(SyntaxNode node)
//    => node is ClassDeclarationSyntax m && m.AttributeLists.Count > 0 && m.AttributeLists.Any(x => x.ContainsAnnotations);
//}


//// */