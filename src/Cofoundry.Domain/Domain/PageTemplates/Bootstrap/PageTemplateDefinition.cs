namespace Cofoundry.Domain;

public class PageTemplateEntityDefinition : IEntityDefinition
{
    public static string DefinitionCode = "COFPTL";

    public string EntityDefinitionCode { get { return DefinitionCode; } }

    public string Name { get { return "Page Template"; } }
}
