namespace Cofoundry.Domain;

/// <summary>
/// Query to get a range of custom entities by a set of ids, projected as a 
/// CustomEntityRenderSummary, which is a general-purpose projection with version 
/// specific data, including a deserialized data model. The results are 
/// version-sensitive and defaults to returning published versions only, but
/// this behavior can be controlled by the publishStatus query property.
/// </summary>
public class GetCustomEntityRenderSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, CustomEntityRenderSummary>>
{
    /// <summary>
    /// Query to get a range of custom entities by a set of ids, projected as a 
    /// CustomEntityRenderSummary, which is a general-purpose projection with version 
    /// specific data, including a deserialized data model. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.
    /// </summary>
    public GetCustomEntityRenderSummariesByIdRangeQuery()
    {
        CustomEntityIds = new List<int>();
    }

    /// <summary>
    /// Query to get a range of custom entities by a set of ids, projected as a 
    /// CustomEntityRenderSummary, which is a general-purpose projection with version 
    /// specific data, including a deserialized data model. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.
    /// </summary>
    /// <param name="customEntityIds">Database ids of the custom entities to get.</param>
    /// <param name="publishStatusQuery">Used to determine which version of the custom entities to include data for.</param>
    public GetCustomEntityRenderSummariesByIdRangeQuery(
        IEnumerable<int>? customEntityIds,
        PublishStatusQuery publishStatusQuery = PublishStatusQuery.Published
        )
        : this(customEntityIds?.ToArray() ?? [], publishStatusQuery)
    {
    }

    /// <summary>
    /// Query to get a range of custom entities by a set of ids, projected as a 
    /// CustomEntityRenderSummary, which is a general-purpose projection with version 
    /// specific data, including a deserialized data model. The results are 
    /// version-sensitive and defaults to returning published versions only, but
    /// this behavior can be controlled by the publishStatus query property.
    /// </summary>
    /// <param name="customEntityIds">Database ids of the custom entities to get.</param>
    /// <param name="publishStatusQuery">Used to determine which version of the custom entities to include data for.</param>
    public GetCustomEntityRenderSummariesByIdRangeQuery(
        IReadOnlyCollection<int> customEntityIds,
        PublishStatusQuery publishStatusQuery = PublishStatusQuery.Published
        )
    {
        ArgumentNullException.ThrowIfNull(customEntityIds);

        CustomEntityIds = customEntityIds;
        PublishStatus = publishStatusQuery;
    }

    /// <summary>
    /// Database ids of the custom entities to get.
    /// </summary>
    [Required]
    public IReadOnlyCollection<int> CustomEntityIds { get; set; }

    /// <summary>
    /// Used to determine which version of the custom entities to include 
    /// data for. This defaults to Published, meaning that only published 
    /// custom entities will be returned.
    /// </summary>
    public PublishStatusQuery PublishStatus { get; set; }
}
