using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user with a specific username address in a specific user area 
    /// returning null if the user could not be found. Note that depending on the
    /// user area, the username may be a copy of the email address.
    /// </summary>
    public class GetUserMicroSummaryByUsernameQueryHandler
        : IAsyncQueryHandler<GetUserMicroSummaryByUsernameQuery, UserMicroSummary>
        , IPermissionRestrictedQueryHandler<GetUserMicroSummaryByUsernameQuery, UserMicroSummary>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUserMicroSummaryByUsernameQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public Task<UserMicroSummary> ExecuteAsync(GetUserMicroSummaryByUsernameQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Username)) return null;

            return Query(query).SingleOrDefaultAsync();
        }

        private IQueryable<UserMicroSummary> Query(GetUserMicroSummaryByUsernameQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Username == query.Username && u.UserAreaCode == query.UserAreaCode)
                .ProjectTo<UserMicroSummary>();
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
