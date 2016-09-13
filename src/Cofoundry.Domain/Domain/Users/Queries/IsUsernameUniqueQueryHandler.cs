using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class IsUsernameUniqueQueryHandler 
        : IQueryHandler<IsUsernameUniqueQuery, bool>
        , IAsyncQueryHandler<IsUsernameUniqueQuery, bool>
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

        public bool Execute(IsUsernameUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = Exists(query).Any();
            return !exists;
        }

        public async Task<bool> ExecuteAsync(IsUsernameUniqueQuery query, IExecutionContext executionContext)
        {
            var exists = await Exists(query).AnyAsync();
            return !exists;
        }

        #endregion

        #region helpers

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
