namespace Cofoundry.Domain;

/// <summary>
/// Entity definition to represent non-cofoundry users which is required
/// to be able to manage thier permissions separately in the system.
/// </summary>
public sealed class NonCofoundryUserEntityDefinition : IEntityDefinition
{
    public const string DefinitionCode = "COFUSN";

    public string EntityDefinitionCode { get { return DefinitionCode; } }

    public string Name { get { return "User (Non Cofoundry)"; } }
}
