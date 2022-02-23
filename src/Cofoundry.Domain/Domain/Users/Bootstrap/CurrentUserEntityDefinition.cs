namespace Cofoundry.Domain
{
    /// <summary>
    /// Entity definition to represent non-cofoundry users which is required
    /// to be able to manage the permissions for the current user differently to
    /// regular users.
    /// </summary>
    public sealed class CurrentUserEntityDefinition : IEntityDefinition
    {
        public const string DefinitionCode = "COFCUR";

        public string EntityDefinitionCode { get { return DefinitionCode; } }

        public string Name { get { return "User (Current)"; } }
    }
}