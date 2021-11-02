using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all access rules associated with a specific page using a default
    /// ordering of specificity i.e. with user area rules before role-based rules.
    /// </summary>
    public class GetPageAccessInfoByPageIdQueryHandler
        : IQueryHandler<GetPageAccessInfoByPageIdQuery, PageAccessInfo>
        , IPermissionRestrictedQueryHandler<GetPageAccessInfoByPageIdQuery, PageAccessInfo>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityAccessInfoMapper _entityAccessInfoMapper;

        public GetPageAccessInfoByPageIdQueryHandler(
            CofoundryDbContext dbContext,
            IEntityAccessInfoMapper entityAccessInfoMapper
            )
        {
            _dbContext = dbContext;
            _entityAccessInfoMapper = entityAccessInfoMapper;
        }

        public async Task<PageAccessInfo> ExecuteAsync(GetPageAccessInfoByPageIdQuery query, IExecutionContext executionContext)
        {
            var dbPage = await _dbContext
                .Pages
                .AsNoTracking()
                .Include(p => p.AccessRules)
                .FilterActive()
                .FilterById(query.PageId)
                .SingleOrDefaultAsync();

            if (dbPage == null) return null;

            var result = new PageAccessInfo();
            await _entityAccessInfoMapper.MapAsync(dbPage, result, executionContext, (dbRule, rule) =>
            {
                rule.PageId = dbRule.PageId;
                rule.PageAccessRuleId = dbRule.PageAccessRuleId;
            });

            result.PageId = dbPage.PageId;
            // TODO: Get inherited rules
            result.InheritedAccessRules = new List<InheritedPageDirectoryAccessInfo>();

            return result;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageAccessInfoByPageIdQuery query)
        {
            yield return new PageReadPermission();
        }
    }
}
