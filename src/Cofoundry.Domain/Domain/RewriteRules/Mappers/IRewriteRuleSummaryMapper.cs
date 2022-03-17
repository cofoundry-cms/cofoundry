using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Simple mapper for mapping to RewriteRuleSummary objects.
/// </summary>
public interface IRewriteRuleSummaryMapper
{
    /// <summary>
    /// Maps an EF RewriteRule record from the db into an RewriteRuleSummary 
    /// object. If the db record is null then null is returned.
    /// </summary>
    /// <param name="dbRewriteRule">RewriteRule record from the database.</param>
    RewriteRuleSummary Map(RewriteRule dbRewriteRule);
}
