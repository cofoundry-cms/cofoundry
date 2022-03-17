using Cofoundry.Domain.Data;

namespace Cofoundry.Domain.Internal;

/// <summary>
/// Gets a complete list of all rewrite rules set up in the system. This result
/// set is cached in memory and quick to access.
/// </summary>
public class GetAllRewriteRuleSummariesQueryHandler
    : IQueryHandler<GetAllRewriteRuleSummariesQuery, ICollection<RewriteRuleSummary>>
    , IPermissionRestrictedQueryHandler<GetAllRewriteRuleSummariesQuery, ICollection<RewriteRuleSummary>>
{
    private readonly CofoundryDbContext _dbContext;
    private readonly IRewriteRuleCache _cache;
    private readonly IRewriteRuleSummaryMapper _rewriteRuleSummaryMapper;

    public GetAllRewriteRuleSummariesQueryHandler(
        CofoundryDbContext dbContext,
        IRewriteRuleCache cache,
        IRewriteRuleSummaryMapper rewriteRuleSummaryMapper
        )
    {
        _dbContext = dbContext;
        _cache = cache;
        _rewriteRuleSummaryMapper = rewriteRuleSummaryMapper;
    }

    public async Task<ICollection<RewriteRuleSummary>> ExecuteAsync(GetAllRewriteRuleSummariesQuery query, IExecutionContext executionContext)
    {
        var rules = await _cache.GetOrAddAsync(async () =>
        {
            var dbResult = await Query().ToListAsync();
            var mapped = dbResult
                .Select(_rewriteRuleSummaryMapper.Map)
                .ToList();

            return mapped;
        });

        return rules;
    }

    private IQueryable<RewriteRule> Query()
    {
        return _dbContext
                .RewriteRules
                .AsNoTracking()
                .OrderByDescending(r => r.CreateDate);
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetAllRewriteRuleSummariesQuery command)
    {
        yield return new RewriteRuleReadPermission();
    }
}