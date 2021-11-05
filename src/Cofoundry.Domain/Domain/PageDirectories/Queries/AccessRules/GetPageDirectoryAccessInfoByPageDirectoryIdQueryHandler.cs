using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Internal;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Returns all access rules associated with a specific page using a default
    /// ordering of specificity i.e. with user area rules before role-based rules.
    /// </summary>
    public class GetPageDirectoryAccessInfoByPageDirectoryIdQueryHandler
        : IQueryHandler<GetPageDirectoryAccessInfoByPageDirectoryIdQuery, PageDirectoryAccessInfo>
        , IPermissionRestrictedQueryHandler<GetPageDirectoryAccessInfoByPageDirectoryIdQuery, PageDirectoryAccessInfo>
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IEntityAccessInfoMapper _entityAccessInfoMapper;
        private readonly IPageDirectoryMicroSummaryMapper _pageDirectoryMicroSummaryMapper;

        public GetPageDirectoryAccessInfoByPageDirectoryIdQueryHandler(
            CofoundryDbContext dbContext,
            IEntityAccessInfoMapper entityAccessInfoMapper,
            IPageDirectoryMicroSummaryMapper pageDirectoryMicroSummaryMapper
            )
        {
            _dbContext = dbContext;
            _entityAccessInfoMapper = entityAccessInfoMapper;
            _pageDirectoryMicroSummaryMapper = pageDirectoryMicroSummaryMapper;
        }

        public async Task<PageDirectoryAccessInfo> ExecuteAsync(GetPageDirectoryAccessInfoByPageDirectoryIdQuery query, IExecutionContext executionContext)
        {
            var dbDirectory = await _dbContext
                .PageDirectories
                .AsNoTracking()
                .Include(d => d.AccessRules)
                .FilterById(query.PageDirectoryId)
                .SingleOrDefaultAsync();

            if (dbDirectory == null) return null;

            var result = new PageDirectoryAccessInfo();
            await _entityAccessInfoMapper.MapAsync(dbDirectory, result, executionContext, (dbRule, rule) =>
            {
                rule.PageDirectoryId = dbRule.PageDirectoryId;
                rule.PageDirectoryAccessRuleId = dbRule.PageDirectoryAccessRuleId;
            });

            result.PageDirectoryId = dbDirectory.PageDirectoryId;
            await MapInheritedRules(dbDirectory, result, executionContext);

            return result;
        }

        private async Task MapInheritedRules(PageDirectory dbPageDirectory, PageDirectoryAccessInfo result, IExecutionContext executionContext)
        {
            var dbInheritedRules = await _dbContext
                .PageDirectoryClosures
                .AsNoTracking()
                .Include(d => d.AncestorPageDirectory)
                .ThenInclude(d => d.AccessRules)
                .Include(d => d.AncestorPageDirectory)
                .ThenInclude(d => d.PageDirectoryPath)
                .FilterByDescendantId(dbPageDirectory.PageDirectoryId)
                .FilterNotSelfReferencing()
                .Where(d => d.DescendantPageDirectoryId == dbPageDirectory.PageDirectoryId && d.AncestorPageDirectory.AccessRules.Any())
                .OrderByDescending(d => d.Distance)
                .ToListAsync();

            result.InheritedAccessRules = new List<InheritedPageDirectoryAccessInfo>();

            foreach (var dbInheritedRule in dbInheritedRules)
            {
                var inheritedDirectory = new InheritedPageDirectoryAccessInfo();
                inheritedDirectory.PageDirectory = _pageDirectoryMicroSummaryMapper.Map(dbInheritedRule.AncestorPageDirectory);
                await _entityAccessInfoMapper.MapAsync(dbInheritedRule.AncestorPageDirectory, inheritedDirectory, executionContext, (dbRule, rule) =>
                {
                    rule.PageDirectoryId = dbRule.PageDirectoryId;
                    rule.PageDirectoryAccessRuleId = dbRule.PageDirectoryAccessRuleId;
                });

                result.InheritedAccessRules.Add(inheritedDirectory);
            }
        }

        public IEnumerable<IPermissionApplication> GetPermissions(GetPageDirectoryAccessInfoByPageDirectoryIdQuery query)
        {
            yield return new PageDirectoryReadPermission();
        }
    }
}
