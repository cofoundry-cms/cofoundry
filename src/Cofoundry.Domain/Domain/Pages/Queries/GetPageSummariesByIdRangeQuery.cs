﻿namespace Cofoundry.Domain;

/// <summary>
/// Finds pages with the specified page ids and returns them as PageSummary 
/// objects. Note that this query does not account for WorkFlowStatus and so
/// pages will be returned irrecpective of whether they aree published or not.
/// </summary>
public class GetPageSummariesByIdRangeQuery : IQuery<IReadOnlyDictionary<int, PageSummary>>
{
    public GetPageSummariesByIdRangeQuery() { }

    /// <summary>
    /// Initializes the query with parameters
    /// </summary>
    /// <param name="pageIds">A collection of database ids of the pages to fetch.</param>
    public GetPageSummariesByIdRangeQuery(IEnumerable<int> pageIds)
        : this(pageIds?.ToArray() ?? [])
    {
    }

    /// <summary>
    /// Initializes the query with parameters.
    /// </summary>
    /// <param name="pageIds">A collection of database ids of the pages to fetch.</param>
    public GetPageSummariesByIdRangeQuery(IReadOnlyCollection<int> pageIds)
    {
        PageIds = pageIds;
    }

    /// <summary>
    /// A collection of database ids of the pages to fetch.
    /// </summary>
    public IReadOnlyCollection<int> PageIds { get; set; } = Array.Empty<int>();
}
