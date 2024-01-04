using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Common mapping functionality for PageSummaries
/// </summary>
public interface IPageSummaryMapper
{
    /// <summary>
    /// Finishes off bulk mapping of tags and page routes in a PageSummary object
    /// </summary>
    Task<IReadOnlyCollection<PageSummary>> MapAsync(IReadOnlyCollection<Page> dbPages, IExecutionContext executionContext);
}
