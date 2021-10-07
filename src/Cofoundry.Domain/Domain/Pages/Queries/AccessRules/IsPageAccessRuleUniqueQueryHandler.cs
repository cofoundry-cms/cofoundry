using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if a page access rule already exists with the specified 
    /// rule configuration. 
    /// </summary>
    public class IsPageAccessRuleUniqueQueryHandler
        : IQueryHandler<IsPageAccessRuleUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsPageAccessRuleUniqueQuery, bool>
    {
        private readonly CofoundryDbContext _dbContext;

        public IsPageAccessRuleUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExecuteAsync(IsPageAccessRuleUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await _dbContext
                .PageAccessRules
                .AsNoTracking()
                .Where(d => d.PageId == query.PageId
                    && d.UserAreaCode == query.UserAreaCode
                    && d.RoleId == query.RoleId
                    && d.PageAccessRuleId != query.PageAccessRuleId
                    ).AnyAsync();

            return !exists;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(IsPageAccessRuleUniqueQuery query)
        {
            yield return new PageReadPermission();
        }
    }

}
