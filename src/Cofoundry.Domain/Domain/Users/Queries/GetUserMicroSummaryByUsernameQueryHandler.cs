using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Finds a user with a specific username address in a specific user area 
    /// returning null if the user could not be found. Note that depending on the
    /// user area, the username may be a copy of the email address.
    /// </summary>
    public class GetUserMicroSummaryByUsernameQueryHandler
        : IQueryHandler<GetUserMicroSummaryByUsernameQuery, UserMicroSummary>
        , IPermissionRestrictedQueryHandler<GetUserMicroSummaryByUsernameQuery, UserMicroSummary>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;

        public GetUserMicroSummaryByUsernameQueryHandler(
            CofoundryDbContext dbContext,
            IUserMicroSummaryMapper userMicroSummaryMapper
            )
        {
            _dbContext = dbContext;
            _userMicroSummaryMapper = userMicroSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<UserMicroSummary> ExecuteAsync(GetUserMicroSummaryByUsernameQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Username)) return null;

            var dbResult = await Query(query).SingleOrDefaultAsync();

            var user = _userMicroSummaryMapper.Map(dbResult);

            return user;
        }

        private IQueryable<User> Query(GetUserMicroSummaryByUsernameQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Username == query.Username && u.UserAreaCode == query.UserAreaCode);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetUserMicroSummaryByUsernameQuery query)
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
