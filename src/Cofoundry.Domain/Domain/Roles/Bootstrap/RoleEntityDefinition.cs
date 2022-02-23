namespace Cofoundry.Domain
{
    /// <summary>
    /// Roles are an assignable collection of permissions. Every user has to be assigned 
    /// to one role.
    /// </summary>
    public sealed class RoleEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFROL";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "Role"; } }
    }
}