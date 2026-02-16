namespace OData.Generators;

[AttributeUsage(AttributeTargets.Class)]
public class GenerateServiceAttribute : Attribute
{
    public GenerateServiceAttribute(string template = null, string typeName = null, string typeNamespace = null, string rootNamespace = null)
    {
        ClassName = typeName;
        Namespace = typeNamespace;
        PrefferredNamespace = rootNamespace;
    }

    public string ClassName { get; set; }
    public string Namespace { get; set; }
    public string PrefferredNamespace { get; set; }
}
