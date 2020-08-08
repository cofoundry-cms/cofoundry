using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Gets a complete list of all rewrite rules set up in the system. This result
    /// set is cached in memory and quick to access.
    /// </summary>
    public class GetAllRewriteRuleSummariesQueryHandler 
        : IQueryHandler<GetAllRewriteRuleSummariesQuery, ICollection<RewriteRuleSummary>>
        , IPermissionRestrictedQueryHandler<GetAllRewriteRuleSummariesQuery, ICollection<RewriteRuleSummary>>
    {
        #region constructor

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

        #endregion

        #region execution

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

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllRewriteRuleSummariesQuery command)
        {
            yield return new RewriteRuleReadPermission();
        }

        #endregion
    }
}
