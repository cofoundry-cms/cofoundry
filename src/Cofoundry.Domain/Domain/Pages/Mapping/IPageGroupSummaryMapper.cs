using Cofoundry.Domain.QueryModels;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to PageGroupSummary objects.
/// </summary>
[Obsolete("The page grouping system will be revised in an upcomming release.")]
public interface IPageGroupSummaryMapper
{
    /// <summary>
    /// Maps an EF PageGroup record from the db into an PageGroupSummary 
    /// object. If the db record is null then null is returned.
    /// </summary>
    /// <param name="queryModel">Query model with data from the database.</param>
    PageGroupSummary? Map(PageGroupSummaryQueryModel? queryModel);
}
