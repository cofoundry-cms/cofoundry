namespace Cofoundry.Domain;

/// <summary>
/// The main entity definition for users. In terms of permissions this
/// represents Cofoundry Admin users.
/// </summary>
public sealed class UserEntityDefinition : IEntityDefinition
{
    public const string DefinitionCode = "COFUSR";

    public string EntityDefinitionCode { get { return DefinitionCode; } }

    public string Name { get { return "User (Cofoundry)"; } }
}
