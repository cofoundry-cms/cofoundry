namespace Cofoundry.Domain;

public sealed class RewriteRuleEntityDefinition : IEntityDefinition
{
    public const string DefinitionCode = "COFRWR";

    public string EntityDefinitionCode { get { return DefinitionCode; } }

    public string Name { get { return "Rewrite Rule"; } }
}
