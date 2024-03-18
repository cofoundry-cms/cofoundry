﻿namespace Cofoundry.Domain.Internal;

/// <summary>
/// Gets a complete list of all rewrite rules set up in the system. This result
/// set is cached in memory and quick to access.
/// </summary>
public class GetRewriteRuleSummaryByPathQueryHandler
    : IQueryHandler<GetRewriteRuleSummaryByPathQuery, RewriteRuleSummary?>
    , IPermissionRestrictedQueryHandler<GetRewriteRuleSummaryByPathQuery, RewriteRuleSummary?>
{
    private readonly IRewriteRuleCache _cache;
    private readonly IQueryExecutor _queryExecutor;

    public GetRewriteRuleSummaryByPathQueryHandler(
        IRewriteRuleCache cache,
        IQueryExecutor queryExecutor
        )
    {
        _cache = cache;
        _queryExecutor = queryExecutor;
    }

    public async Task<RewriteRuleSummary?> ExecuteAsync(GetRewriteRuleSummaryByPathQuery query, IExecutionContext executionContext)
    {
        var rules = await _queryExecutor.ExecuteAsync(new GetAllRewriteRuleSummariesQuery(), executionContext);
        return FindRule(query, rules);
    }

    private RewriteRuleSummary? FindRule(GetRewriteRuleSummaryByPathQuery query, IReadOnlyCollection<RewriteRuleSummary> rules)
    {
        RewriteRuleSummary? rule = null;
        var path = query.Path;

        // Set up an alternate path so we can check with/without a trailing slash 
        string[] pathVariations;
        if (string.IsNullOrEmpty(Path.GetExtension(path)))
        {
            var alternatePath = path.EndsWith("/") ? path.Substring(0, path.Length - 1) : path + "/";
            pathVariations = [path, alternatePath];
        }
        else
        {
            pathVariations = [path];
        }


        // Check for paths
        foreach (var pathVariation in pathVariations)
        {
            rule = rules
                .FirstOrDefault(r => r.WriteFrom.Equals(pathVariation, StringComparison.OrdinalIgnoreCase)
                || (r.WriteFrom.EndsWith("*") && pathVariation.StartsWith(r.WriteFrom.Substring(0, r.WriteFrom.Length - 1), StringComparison.OrdinalIgnoreCase)));

            if (rule != null)
            {
                break;
            }
        }

        if (rule == null)
        {
            return null;
        }

        // Check to make sure we are not redirecting to the same path
        foreach (var pathVariation in pathVariations)
        {
            if (rule.WriteTo.Equals(pathVariation, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
        }

        return rule;
    }

    public IEnumerable<IPermissionApplication> GetPermissions(GetRewriteRuleSummaryByPathQuery command)
    {
        yield return new RewriteRuleReadPermission();
    }
}