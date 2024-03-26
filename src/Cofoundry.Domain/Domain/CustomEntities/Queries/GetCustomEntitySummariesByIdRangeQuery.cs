namespace Cofoundry.Domain;

/// <summary>
/// An id range query for custom entities which returns basic
/// custom entity information with publish status and model data for the
/// latest version. The query is not version-sensitive and is designed to be 
/// used in the admin panel and not in a version-sensitive context such as a 
/// public webpage.
/// </summary>
public class GetCustomEntitySummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, CustomEntitySummary>>
{
    /// <summary>
    /// An id range query for custom entities which returns basic
    /// custom entity information with workflow status and model data for the
    /// latest version. The query is not version-sensitive and is designed to be 
    /// used in the admin panel and not in a version-sensitive context such as a 
    /// public webpage.
    /// </summary>
    public GetCustomEntitySummariesByIdRangeQuery()
    {
        CustomEntityIds = new List<int>();
    }

    /// <summary>
    /// An id range query for custom entities which returns basic
    /// custom entity information with publish status and model data for the
    /// latest version. The query is not version-sensitive and is designed to be 
    /// used in the admin panel and not in a version-sensitive context such as a 
    /// public webpage.
    /// </summary>
    /// <param name="customEntityIds">
    /// Collection of custom entity ids to find. Any ids unable to be
    /// located will not be present in the result.
    /// </param>
    public GetCustomEntitySummariesByIdRangeQuery(
        IEnumerable<int>? customEntityIds
        )
        : this(customEntityIds?.ToArray() ?? [])
    {
    }

    /// <summary>
    /// An id range query for custom entities which returns basic
    /// custom entity information with publish status and model data for the
    /// latest version. The query is not version-sensitive and is designed to be 
    /// used in the admin panel and not in a version-sensitive context such as a 
    /// public webpage.
    /// </summary>
    /// <param name="customEntityIds">
    /// Collection of custom entity ids to find. Any ids unable to be
    /// located will not be present in the result.
    /// </param>
    public GetCustomEntitySummariesByIdRangeQuery(
        IReadOnlyCollection<int> customEntityIds
        )
    {
        ArgumentNullException.ThrowIfNull(customEntityIds);

        CustomEntityIds = customEntityIds;
    }

    /// <summary>
    /// Collection of custom entity ids to find. Any ids unable to be
    /// located will not be present in the result.
    /// </summary>
    [Required]
    public IReadOnlyCollection<int> CustomEntityIds { get; set; }
}
