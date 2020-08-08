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
    /// Finds a user with a specific email address in a specific user area 
    /// returning null if the user could not be found. Note that if the user
    /// area does not support email addresses then the email field will be empty.
    /// </summary>
    public class GetUserMicroSummaryByEmailQueryHandler 
        : IQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
        , IPermissionRestrictedQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;
        private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;

        public GetUserMicroSummaryByEmailQueryHandler(
            CofoundryDbContext dbContext,
            IUserMicroSummaryMapper userMicroSummaryMapper
            )
        {
            _dbContext = dbContext;
            _userMicroSummaryMapper = userMicroSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<UserMicroSummary> ExecuteAsync(GetUserMicroSummaryByEmailQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Email)) return null;

            var dbResult = await Query(query).SingleOrDefaultAsync();
            var user = _userMicroSummaryMapper.Map(dbResult);

            return user;
        }

        private IQueryable<User> Query(GetUserMicroSummaryByEmailQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Email == query.Email && u.UserAreaCode == query.UserAreaCode);
        }

        #endregion

        #region permissions

        public IEnumerable<IPermissionApplication> GetPermissions(GetUserMicroSummaryByEmailQuery query)
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
