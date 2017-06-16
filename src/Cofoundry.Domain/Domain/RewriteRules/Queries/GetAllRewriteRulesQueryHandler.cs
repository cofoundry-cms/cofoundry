using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a complete list of all rewrite rules set up in the system. This result
    /// set is cached in memory and quick to access.
    /// </summary>
    public class GetAllRewriteRulesQueryHandler 
        : IQueryHandler<GetAllQuery<RewriteRuleSummary>, IEnumerable<RewriteRuleSummary>>
        , IAsyncQueryHandler<GetAllQuery<RewriteRuleSummary>, IEnumerable<RewriteRuleSummary>>
        , IPermissionRestrictedQueryHandler<GetAllQuery<RewriteRuleSummary>, IEnumerable<RewriteRuleSummary>>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IRewriteRuleCache _cache;

        public GetAllRewriteRulesQueryHandler(
            CofoundryDbContext dbContext,
            IRewriteRuleCache cache
            )
        {
            _dbContext = dbContext;
            _cache = cache;
        }

        #endregion

        #region execution

        public IEnumerable<RewriteRuleSummary> Execute(GetAllQuery<RewriteRuleSummary> query, IExecutionContext executionContext)
        {
            var rules = _cache.GetOrAdd(() =>
            {
                return Query().ToArray();
            });

            return rules;
        }

        public async Task<IEnumerable<RewriteRuleSummary>> ExecuteAsync(GetAllQuery<RewriteRuleSummary> query, IExecutionContext executionContext)
        {
            var rules = await _cache.GetOrAddAsync(() =>
            {
                return Query().ToArrayAsync();
            });

            return rules;
        }

        private IQueryable<RewriteRuleSummary> Query()
        {
            return _dbContext
                    .RewriteRules
                    .AsNoTracking()
                    .OrderByDescending(r => r.CreateDate)
                    .ProjectTo<RewriteRuleSummary>();
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetAllQuery<RewriteRuleSummary> command)
        {
            yield return new RewriteRuleReadPermission();
        }

        #endregion
    }
}
