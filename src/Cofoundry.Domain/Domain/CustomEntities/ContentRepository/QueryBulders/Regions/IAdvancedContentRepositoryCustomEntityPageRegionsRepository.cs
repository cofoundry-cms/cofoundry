namespace Cofoundry.Domain;

/// <summary>
/// Queries and commands for template regions data in a custom entity page.
/// </summary>
public interface IAdvancedContentRepositoryCustomEntityPageRegionsRepository
{
    /// <summary>
    /// Queries and commands for block data on a custom entity page.
    /// </summary>
    IAdvancedContentRepositoryCustomEntityPageBlocksRepository Blocks();
}
