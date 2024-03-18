﻿namespace Cofoundry.Domain;

/// <summary>
/// Finds routing information for a custom entitiy by it's id. Although
/// in a typical website you wouldn't have multiple details pages for a custom entity
/// type, it is supported and the query returns a collection of routes.
/// </summary>
public class GetPageRoutingInfoByCustomEntityIdQuery : IQuery<IReadOnlyCollection<PageRoutingInfo>>
{
    /// <summary>
    /// Finds routing information for a custom entitiy by it's id. Although
    /// in a typical website you wouldn't have multiple details pages for a custom entity
    /// type, it is supported and the query returns a collection of routes.
    /// </summary>
    public GetPageRoutingInfoByCustomEntityIdQuery()
    {
    }

    /// <summary>
    /// Finds routing information for a custom entitiy by it's id. Although
    /// in a typical website you wouldn't have multiple details pages for a custom entity
    /// type, it is supported and the query returns a collection of routes.
    /// </summary>
    /// <param name="customEntityId">Database id of the custom entity to find routing data for.</param>
    public GetPageRoutingInfoByCustomEntityIdQuery(
        int customEntityId
        )
    {
        CustomEntityId = customEntityId;
    }

    /// <summary>
    /// Database id of the custom entity to find routing data for.
    /// </summary>
    public int CustomEntityId { get; set; }
}
