﻿namespace Cofoundry.Domain;

/// <summary>
/// Returns custom entities filtered on the url slug value. This query
/// can return multiple custom entities because unique url slugs are an
/// optional setting on the custom entity definition.
/// </summary>
public class GetCustomEntityRenderSummariesByUrlSlugQuery : IQuery<IReadOnlyCollection<CustomEntityRenderSummary>>
{
    /// <summary>
    /// Returns custom entities filtered on the url slug value. This query
    /// can return multiple custom entities because unique url slugs are an
    /// optional setting on the custom entity definition.
    /// </summary>
    public GetCustomEntityRenderSummariesByUrlSlugQuery()
    {
    }

    /// <summary>
    /// Returns custom entities filtered on the url slug value. This query
    /// can return multiple custom entities because unique url slugs are an
    /// optional setting on the custom entity definition.
    /// </summary>
    /// <param name="customEntityDefinitionCode">Required. The definition code of the custom entity to filter on.</param>
    /// <param name="urlSlug">Required. The url slug to find matching custom entities for.</param>
    /// <param name="publishStatusQuery">
    /// Used to determine which version of the custom entities to include data for. This 
    /// defaults to Published, meaning that only published custom entities will be returned.
    /// Note that PublishStatusQuery.SpecificVersion is not available
    /// because the query can potentially return multiple results.
    /// </param>
    public GetCustomEntityRenderSummariesByUrlSlugQuery(
        string customEntityDefinitionCode,
        string urlSlug,
        PublishStatusQuery publishStatusQuery = PublishStatusQuery.Published
        )
    {
        CustomEntityDefinitionCode = customEntityDefinitionCode;
        UrlSlug = urlSlug;
        PublishStatus = publishStatusQuery;
    }

    /// <summary>
    /// Required. The definition code of the custom entity to filter on.
    /// </summary>
    [Required]
    public string CustomEntityDefinitionCode { get; set; } = string.Empty;

    /// <summary>
    /// Required. The url slug to find matching custom entities for.
    /// </summary>
    [Required]
    public string UrlSlug { get; set; } = string.Empty;

    /// <summary>
    /// Used to determine which version of the custom entities to include data for. This 
    /// defaults to Published, meaning that only published custom entities will be returned.
    /// Note that PublishStatusQuery.SpecificVersion is not available
    /// because the query can potentially return multiple results.
    /// </summary>
    public PublishStatusQuery PublishStatus { get; set; }
}
