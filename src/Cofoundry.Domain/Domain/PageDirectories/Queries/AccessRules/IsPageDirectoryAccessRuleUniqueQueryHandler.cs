using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Determines if a page directory access rule already exists with the specified 
    /// rule configuration. 
    /// </summary>
    public class IsPageDirectoryAccessRuleUniqueQueryHandler
        : IQueryHandler<IsPageDirectoryAccessRuleUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsPageDirectoryAccessRuleUniqueQuery, bool>
    {
        private readonly CofoundryDbContext _dbContext;

        public IsPageDirectoryAccessRuleUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        public async Task<bool> ExecuteAsync(IsPageDirectoryAccessRuleUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await _dbContext
                .PageDirectoryAccessRules
                .AsNoTracking()
                .Where(d => d.PageDirectoryId == query.PageDirectoryId
                    && d.UserAreaCode == query.UserAreaCode
                    && d.RoleId == query.RoleId
                    && d.PageDirectoryAccessRuleId != query.PageDirectoryAccessRuleId
                    ).AnyAsync();

            return !exists;
        }

        public IEnumerable<IPermissionApplication> GetPermissions(IsPageDirectoryAccessRuleUniqueQuery query)
        {
            yield return new PageDirectoryReadPermission();
        }
    }

}
