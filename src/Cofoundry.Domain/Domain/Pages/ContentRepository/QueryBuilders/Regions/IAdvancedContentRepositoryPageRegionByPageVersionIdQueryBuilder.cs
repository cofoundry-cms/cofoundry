﻿namespace Cofoundry.Domain;

/// <summary>
/// Queries for retrieving regions and blocks for a specific verison of a page.
/// </summary>
public interface IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder
{
    /// <summary>
    /// Query retruning a collection of content managed regions with
    /// block data for a specific version of a page.
    /// </summary>
    IDomainRepositoryQueryContext<IReadOnlyCollection<PageRegionDetails>> AsDetails();
}
