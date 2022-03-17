namespace Cofoundry.Domain;

public interface IEntityDefinition
{
    /// <summary>
    /// Unique 6 character code representing the entity (use uppercase).
    /// </summary>
    string EntityDefinitionCode { get; }

    /// <summary>
    /// Singlar name of the entity e.g. 'Page'
    /// </summary>
    string Name { get; }
}
