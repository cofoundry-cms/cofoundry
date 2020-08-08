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
    /// Finds a user by a database id returning a UserMicroSummary object if it 
    /// is found, otherwise null.
    /// </summary>
    public class GetUserMicroSummaryByIdQueryHandler 
        : IQueryHandler<GetUserMicroSummaryByIdQuery, UserMicroSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserMicroSummaryMapper _userMicroSummaryMapper;

        public GetUserMicroSummaryByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService,
            IUserMicroSummaryMapper userMicroSummaryMapper
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
            _userMicroSummaryMapper = userMicroSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<UserMicroSummary> ExecuteAsync(GetUserMicroSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await Query(query).SingleOrDefaultAsync();
            var user = _userMicroSummaryMapper.Map(dbResult);

            ValidatePermission(query, executionContext, user);

            return user;
        }

        private IQueryable<User> Query(GetUserMicroSummaryByIdQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .Where(u => u.UserId == query.UserId);
        }

        private void ValidatePermission(GetUserMicroSummaryByIdQuery query, IExecutionContext executionContext, UserMicroSummary user)
        {
            if (user == null) return;

            if (user.UserArea.UserAreaCode == CofoundryAdminUserArea.AreaCode)
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<CofoundryUserReadPermission>(query.UserId, executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<NonCofoundryUserReadPermission>(query.UserId, executionContext.UserContext);
            }
        }

        #endregion
    }
}
