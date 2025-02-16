using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace OData.Generators;

public class AttributeSyntaxReceiver<TAttribute> : ISyntaxReceiver
   where TAttribute : Attribute
{
    public const string AttributePostFix = "Attribute";

    public IList<ClassDeclarationSyntax> Classes { get; } = new List<ClassDeclarationSyntax>();

    public List<string> Log { get; set; } = new List<string>();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
            && GetFrom(classDeclarationSyntax) != null
            )
        {
            Log.Add(classDeclarationSyntax.Identifier.ValueText);
            foreach (var item in classDeclarationSyntax.AttributeLists)
            {
                foreach (var a in item.Attributes)
                {
                    Log.Add($"\t{a.Name}");
                }
            }

            Classes.Add(classDeclarationSyntax);
        }
    }
    public AttributeSyntax GetFrom(ClassDeclarationSyntax classDeclarationSyntax)
    {
        var attributeType = typeof(TAttribute);
        var attributeNames = new[] { attributeType.Name, attributeType.Name.Replace(AttributePostFix, ""), attributeType.FullName, attributeType.FullName.Replace(AttributePostFix, "") };

        return classDeclarationSyntax.AttributeLists
                .SelectMany(sm => sm.Attributes)
                .FirstOrDefault(a => attributeNames.Contains(a.Name.ToString()));
        //.First(x => x.Name.ToString().EnsureEndsWith("Attribute").Equals(typeof(GenerateServiceAttribute).Name));
        //classDeclarationSyntax.AttributeLists.Count > 0 &&
        //    classDeclarationSyntax.AttributeLists
        //        .Any(al => al.Attributes.Any(a => a.Name.ToString().EnsureEndsWith(AttributePostFix).Equals(typeof(TAttribute).Name)))
    }

    /// <summary>
    /// https://stackoverflow.com/questions/70402988/how-to-completely-evaluate-an-attributes-parameters-in-a-c-sharp-source-generat
    /// </summary>
    /// <example>
    /// if (syntaxNode is AttributeSyntax attributeSyntax)
    /// {
    ///     var collector = new AttributeCollector("My", "MyAttribute");
    ///     attributeSyntax.Accept(collector);
    ///     AttributeDefinitions.AddRange(collector.AttributeDefinitions);
    /// }
    /// </example>
    internal class AttributeCollector : CSharpSyntaxVisitor
    {
        private readonly HashSet<string> attributeNames;

        public List<AttributeDefinition> AttributeDefinitions { get; } = new();

        public AttributeCollector(params string[] attributeNames)
        {
            this.attributeNames = new HashSet<string>(attributeNames);
        }

        public override void VisitAttribute(AttributeSyntax node)
        {
            base.VisitAttribute(node);

            if (!attributeNames.Contains(node.Name.ToString()))
            {
                return;
            }

            var fieldArguments = new List<(string Name, object Value)>();
            var propertyArguments = new List<(string Name, object Value)>();

            var arguments = node.ArgumentList?.Arguments.ToArray() ?? Array.Empty<AttributeArgumentSyntax>();
            foreach (var syntax in arguments)
            {
                if (syntax.NameColon != null)
                {
                    fieldArguments.Add((syntax.NameColon.Name.ToString(), syntax.Expression));
                }
                else if (syntax.NameEquals != null)
                {
                    propertyArguments.Add((syntax.NameEquals.Name.ToString(), syntax.Expression));
                }
                else
                {
                    fieldArguments.Add((string.Empty, syntax.Expression));
                }
            }

            AttributeDefinitions.Add(new AttributeDefinition
            {
                Name = node.Name.ToString(),
                FieldArguments = fieldArguments.ToArray(),
                PropertyArguments = propertyArguments.ToArray()
            });
        }
    }

    internal record AttributeDefinition
    {
        public string Name { get; set; }
        public (string Name, object Value)[] FieldArguments { get; set; } = Array.Empty<(string Name, object Value)>();
        public (string Name, object Value)[] PropertyArguments { get; set; } = Array.Empty<(string Name, object Value)>();

        public string ToSource()
        {
            var definition = new StringBuilder(Name);
            if (!FieldArguments.Any() && !PropertyArguments.Any())
            {
                return definition.ToString();
            }

            return definition
                .Append("(")
                .Append(ArgumentsToString())
                .Append(")")
                .ToString();
        }

        private string ArgumentsToString()
        {
            var arguments = new StringBuilder();

            if (FieldArguments.Any())
            {
                arguments.Append(string.Join(", ", FieldArguments.Select(
                    param => string.IsNullOrEmpty(param.Name)
                        ? $"{param.Value}"
                        : $"{param.Name}: {param.Value}")
                ));
            }

            if (PropertyArguments.Any())
            {
                arguments
                    .Append(arguments.Length > 0 ? ", " : "")
                    .Append(string.Join(", ", PropertyArguments.Select(
                        param => $"{param.Name} = {param.Value}")
                    ));
            }

            return arguments.ToString();
        }
    }
}
