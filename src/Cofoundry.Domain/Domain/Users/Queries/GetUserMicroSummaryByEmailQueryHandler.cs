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
    /// Finds a user with a specific email address in a specific user area 
    /// returning null if the user could not be found. Note that if the user
    /// area does not support email addresses then the email field will be empty.
    /// </summary>
    public class GetUserMicroSummaryByEmailQueryHandler 
        : IQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
        , IAsyncQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
        , IPermissionRestrictedQueryHandler<GetUserMicroSummaryByEmailQuery, UserMicroSummary>
    {
        #region constructor

        private readonly CofoundryDbContext _dbContext;

        public GetUserMicroSummaryByEmailQueryHandler(
            CofoundryDbContext dbContext
            )
        {
            _dbContext = dbContext;
        }

        #endregion

        #region execution

        public UserMicroSummary Execute(GetUserMicroSummaryByEmailQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Email)) return null;

            var user = Query(query).SingleOrDefault();

            return user;
        }

        public Task<UserMicroSummary> ExecuteAsync(GetUserMicroSummaryByEmailQuery query, IExecutionContext executionContext)
        {
            if (string.IsNullOrWhiteSpace(query.Email)) return null;

            return Query(query).SingleOrDefaultAsync();
        }

        private IQueryable<UserMicroSummary> Query(GetUserMicroSummaryByEmailQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.Email == query.Email && u.UserAreaCode == query.UserAreaCode)
                .ProjectTo<UserMicroSummary>();
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
