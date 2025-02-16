using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using OData.Generators.Templates;

namespace OData.Generators;

[Generator]
public class ServiceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        //#if DEBUG
        //        if (!Debugger.IsAttached){Debugger.Launch();}
        //#endif
        Debug.WriteLine("Initalize code generator");

        context.RegisterForSyntaxNotifications(() => new AttributeSyntaxReceiver<GenerateServiceAttribute>());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        Debug.WriteLine("Execute code generator");

        var sb = new StringBuilder();
        sb.AppendLine("/*");
        sb.AppendLine($"{DateTime.Now.ToShortTimeString()}");

        if (context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateServiceAttribute> syntaxReceiver)
        {
            sb.AppendLine("context.SyntaxReceiver is not AttributeSyntaxReceiver<GenerateServiceAttribute>");
            syntaxReceiver = new AttributeSyntaxReceiver<GenerateServiceAttribute>();

            //foreach (var item in context.Compilation.SourceModule.ReferencedAssemblies){ sb.AppendLine($"{item.Name}"); }
            //context.AddSource($"Log", SourceText.From(sb.ToString(), Encoding.UTF8));
            /*
            var types = context.Compilation.SourceModule.ReferencedAssemblySymbols.SelectMany(a =>
            {
                try
                {
                    var main = a.Identity.Name.Split('.').Aggregate(a.GlobalNamespace, (s, c) => s.GetNamespaceMembers().Single(m => m.Name.Equals(c)));

                    return GetAllTypes(main);
                }
                catch { return Enumerable.Empty<ITypeSymbol>(); }
            });

            foreach (var item in types.SelectMany(x => x.DeclaringSyntaxReferences.Select(x => x.GetSyntax())).ToList())
            {
                syntaxReceiver.OnVisitSyntaxNode(item);
            }
            */
        }

        sb.AppendLine(string.Join("\r\n", syntaxReceiver.Log));
        sb.AppendLine($"loop START, Any: {syntaxReceiver.Classes?.Any()}");
        foreach (var classSyntax in syntaxReceiver.Classes)
        {
            sb.AppendLine("foreach");

            // Converting the class to semantic model to access much more meaningful data.
            var model = context.Compilation.GetSemanticModel(classSyntax.SyntaxTree);
            // Parse to declared symbol, so you can access each part of code separately, such as interfaces, methods, members, contructor parameters etc.
            var symbol = model.GetDeclaredSymbol(classSyntax);

            // Finding my GenerateServiceAttribute over it. I'm sure this attribute is placed, because my syntax receiver already checked before.
            // So, I can surely execute following query.
            var attribute = syntaxReceiver.GetFrom(classSyntax);

            // Getting constructor parameter of the attribute. It might be not presented.
            var templateParameter = attribute.ArgumentList?.Arguments.FirstOrDefault()?.GetLastToken().ValueText; // Temprorary... Attribute has only one argument for now.
            var className = attribute.ArgumentList?.Arguments.Count > 1 ?
                    attribute.ArgumentList?.Arguments.Skip(1).FirstOrDefault()?.GetLastToken().ValueText : null;

            // Can't access embeded resource of main project.
            // So overridden template must be marked as Analyzer Additional File to be able to be accessed by an analyzer.
            var overridenTemplate = templateParameter != null ?
                context.AdditionalFiles.FirstOrDefault(x => x.Path.EndsWith(templateParameter))?.GetText().ToString() :
                null;

            // Generate the real source code. Pass the template parameter if there is a overriden template.
            var sourceCode = GetSourceCodeFor(symbol, overridenTemplate, className);

            context.AddSource(
                $"{symbol.Name}{templateParameter ?? "Controller"}.g.cs",
                SourceText.From(sourceCode, Encoding.UTF8));

            Console.WriteLine(classSyntax);
        }
        sb.AppendLine("loop END");

        sb.AppendLine("*/");
        context.AddSource($"Log", SourceText.From(sb.ToString(), Encoding.UTF8));
    }

    private string GetSourceCodeFor(INamedTypeSymbol symbol, string template = null, string className = null)
    {
        // If template isn't provieded, use default one from embeded resources.
        template ??= GetEmbededResource("OData.Generators.Templates.Default.txt");

        // Can't use scriban at the moment, make it manually for now.
        return template
            .Replace("{{" + nameof(DefaultTemplateParameters.ClassName) + "}}", className ?? symbol.Name)
            .Replace("{{" + nameof(DefaultTemplateParameters.Namespace) + "}}", GetNamespaceRecursively(symbol.ContainingNamespace))
            .Replace("{{" + nameof(DefaultTemplateParameters.PrefferredNamespace) + "}}", symbol.ContainingAssembly.Name)
            ;
    }

    private string GetEmbededResource(string path)
    {
        using var stream = GetType().Assembly.GetManifestResourceStream(path);

        using var streamReader = new StreamReader(stream);

        return streamReader.ReadToEnd();
    }

    private string GetNamespaceRecursively(INamespaceSymbol symbol)
    {
        if (symbol.ContainingNamespace == null)
        {
            return symbol.Name;
        }

        return (GetNamespaceRecursively(symbol.ContainingNamespace) + "." + symbol.Name).Trim('.');
    }

    private static IEnumerable<ITypeSymbol> GetAllTypes(INamespaceSymbol root)
    {
        foreach (var namespaceOrTypeSymbol in root.GetMembers())
        {
            if (namespaceOrTypeSymbol is INamespaceSymbol @namespace)
            {
                foreach (var nested in GetAllTypes(@namespace))
                {
                    yield return nested;
                }
            }
            else if (namespaceOrTypeSymbol is ITypeSymbol type)
            {
                yield return type;
            }
        }
    }
}
