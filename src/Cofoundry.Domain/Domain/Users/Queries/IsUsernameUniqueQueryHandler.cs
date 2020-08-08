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
    /// Determines if a username is unique within a specific UserArea.
    /// Usernames only have to be unique per UserArea.
    /// </summary>
    public class IsUsernameUniqueQueryHandler 
        : IQueryHandler<IsUsernameUniqueQuery, bool>
        , IPermissionRestrictedQueryHandler<IsUsernameUniqueQuery, bool>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public IsUsernameUniqueQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }
        
        #endregion

        #region execution

        public async Task<bool> ExecuteAsync(IsUsernameUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await Exists(query).AnyAsync();
            return !exists;
        }

        private IQueryable<User> Exists(IsUsernameUniqueQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .FilterActive()
                .FilterByUserArea(query.UserAreaCode)
                .Where(u => u.UserId != query.UserId && u.Username == query.Username);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(IsUsernameUniqueQuery query)
        {
            if (query.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                yield return new CofoundryUserReadPermission();
            }
            else
            {
                yield return new NonCofoundryUserReadPermission();
            }
        }

        #endregion
    }

}
