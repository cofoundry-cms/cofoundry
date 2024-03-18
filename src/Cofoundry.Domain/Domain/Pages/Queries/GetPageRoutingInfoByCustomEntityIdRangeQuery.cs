﻿namespace Cofoundry.Domain;

/// <summary>
/// Finds routing information for a set of custom entities by their ids. Although
/// in a typical website you wouldn't have multiple details pages for a custom entity
/// type, it is supported and so each custom entity id in the query returns a collection
/// of routes.
/// </summary>
public class GetPageRoutingInfoByCustomEntityIdRangeQuery : IQuery<IReadOnlyDictionary<int, IReadOnlyCollection<PageRoutingInfo>>>
{
    /// <summary>
    /// Finds routing information for a set of custom entities by their ids. Although
    /// in a typical website you wouldn't have multiple details pages for a custom entity
    /// type, it is supported and so each custom entity id in the query returns a collection
    /// of routes.
    /// </summary>
    public GetPageRoutingInfoByCustomEntityIdRangeQuery() { }

    /// <summary>
    /// Finds routing information for a set of custom entities by their ids. Although
    /// in a typical website you wouldn't have multiple details pages for a custom entity
    /// type, it is supported and so each custom entity id in the query returns a collection
    /// of routes.
    /// </summary>
    /// <param name="customEntityIds">Database ids of the custom entities to find routing data for.</param>
    public GetPageRoutingInfoByCustomEntityIdRangeQuery(IEnumerable<int> customEntityIds)
        : this(customEntityIds?.ToArray() ?? [])
    {
    }

    /// <summary>
    /// Finds routing information for a set of custom entities by their ids. Although
    /// in a typical website you wouldn't have multiple details pages for a custom entity
    /// type, it is supported and so each custom entity id in the query returns a collection
    /// of routes.
    /// </summary>
    /// <param name="customEntityIds">Database ids of the custom entities to find routing data for.</param>
    public GetPageRoutingInfoByCustomEntityIdRangeQuery(IReadOnlyCollection<int> customEntityIds)
    {
        CustomEntityIds = customEntityIds;
    }

    /// <summary>
    /// Database ids of the custom entities to find routing data for.
    /// </summary>
    public IReadOnlyCollection<int> CustomEntityIds { get; set; } = Array.Empty<int>();
}
