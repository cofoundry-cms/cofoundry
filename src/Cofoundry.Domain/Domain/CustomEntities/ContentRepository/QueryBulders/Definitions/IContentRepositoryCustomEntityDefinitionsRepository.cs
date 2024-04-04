namespace Cofoundry.Domain;

/// <summary>
/// Queries and commands for custom entity definitions.
/// </summary>
public interface IContentRepositoryCustomEntityDefinitionsRepository
{
    /// <summary>
    /// Get a custom entity definition by its unique 6 character code.
    /// </summary>
    /// <param name="customEntityDefinitionCode">Unique 6 letter code representing the entity.</param>
    IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder GetByCode(string customEntityDefinitionCode);

    /// <summary>
    /// Get all custom entity definitions.
    /// </summary>
    IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder GetAll();
}
