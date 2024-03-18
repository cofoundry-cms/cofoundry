﻿namespace Cofoundry.Domain;

/// <summary>
/// Queries to return a collection of all ICustomEntityRoutingRule implementations
/// registered with the DI system.
/// </summary>
public interface IAdvancedContentRepositoryGetAllCustomEntityRoutingRuleQueryBuilder
{
    /// <summary>
    /// Returns all instances of ICustomEntityRoutingRule registered
    /// in the application.
    /// </summary>
    IDomainRepositoryQueryContext<IReadOnlyCollection<ICustomEntityRoutingRule>> AsRoutingRules();
}
