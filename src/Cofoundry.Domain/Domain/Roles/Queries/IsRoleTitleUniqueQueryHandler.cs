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
    /// Determines if a role title is unique within a specific UserArea.
    /// Role titles only have to be unique per UserArea.
    /// </summary>
    public class IsRoleTitleUniqueQueryHandler 
        : IQueryHandler<IsRoleTitleUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsRoleTitleUniqueQuery, bool>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public IsRoleTitleUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }
        
        #endregion

        #region execution

        public async Task<bool> ExecuteAsync(IsRoleTitleUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await Exists(query).AnyAsync();
            return !exists;
        }

        #endregion

        #region helpers

        private IQueryable<Role> Exists(IsRoleTitleUniqueQuery query)
        {
            return _dbContext
                .Roles
                .AsNoTracking()
                .Where(r => r.RoleId != query.RoleId && r.Title == query.Title && r.UserAreaCode == query.UserAreaCode);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(IsRoleTitleUniqueQuery command)
        {
            yield return new RoleReadPermission();
        }

        #endregion
    }

}
