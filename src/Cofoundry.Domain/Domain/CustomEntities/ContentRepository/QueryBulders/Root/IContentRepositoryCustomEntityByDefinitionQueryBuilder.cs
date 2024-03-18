﻿namespace Cofoundry.Domain;

/// <summary>
/// Queries for retrieving all custom entities of a specific type (definition).
/// </summary>
public interface IContentRepositoryCustomEntityByDefinitionQueryBuilder
{
    /// <summary>
    /// Queries all custom entites of a specific type, projected as a
    /// <see cref="CustomEntityRenderSummary"/>, which is a general-purpose projection with version 
    /// specific data, including a deserialized data model. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query parameter.
    /// </summary>
    /// <param name="publishStatusQuery">
    /// Used to determine which version of the custom entities to include data for. This 
    /// defaults to Published, meaning that only published custom entities will be returned.
    /// </param>
    IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery? publishStatusQuery = null);

    /// <summary>
    /// Queries <see cref="CustomEntityRoute"/> data for all custom entities of a 
    /// specific type. These route objects are small and cached which
    /// makes them good for quick lookups.
    /// </summary>
    IDomainRepositoryQueryContext<IReadOnlyCollection<CustomEntityRoute>> AsRoutes();
}
