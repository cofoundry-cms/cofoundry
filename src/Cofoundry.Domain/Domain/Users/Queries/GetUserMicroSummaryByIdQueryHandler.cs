using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;
using AutoMapper.QueryableExtensions;
using System.Data.Entity;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user by a database id returning a UserMicroSummary object if it 
    /// is found, otherwise null.
    /// </summary>
    public class GetUserMicroSummaryByIdQueryHandler 
        : IQueryHandler<GetByIdQuery<UserMicroSummary>, UserMicroSummary>
        , IAsyncQueryHandler<GetByIdQuery<UserMicroSummary>, UserMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUserMicroSummaryByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        #endregion

        #region execution

        public UserMicroSummary Execute(GetByIdQuery<UserMicroSummary> query, IExecutionContext executionContext)
        {
            var user = Query(query).SingleOrDefault();
            ValidatePermission(query, executionContext, user);

            return user;
        }

        public async Task<UserMicroSummary> ExecuteAsync(GetByIdQuery<UserMicroSummary> query, IExecutionContext executionContext)
        {
            var user = await Query(query).SingleOrDefaultAsync();
            ValidatePermission(query, executionContext, user);

            return user;
        }

        private IQueryable<UserMicroSummary> Query(GetByIdQuery<UserMicroSummary> query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.UserId == query.Id)
                .ProjectTo<UserMicroSummary>();
        }

        private void ValidatePermission(GetByIdQuery<UserMicroSummary> query, IExecutionContext executionContext, UserMicroSummary user)
        {
            if (user == null) return;

            if (user.UserArea.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<CofoundryUserReadPermission>(query.Id, executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<NonCofoundryUserReadPermission>(query.Id, executionContext.UserContext);
            }
        }

        #endregion
    }
}
