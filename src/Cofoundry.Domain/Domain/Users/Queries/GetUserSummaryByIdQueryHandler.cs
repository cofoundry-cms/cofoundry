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
    public class GetUserSummaryByIdQueryHandler 
        : IQueryHandler<GetUserSummaryByIdQuery, UserSummary>
        , IIgnorePermissionCheckHandler
    {
        #region constructor
        
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserSummaryMapper _userSummaryMapper;

        public GetUserSummaryByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService,
            IUserSummaryMapper userSummaryMapper
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
            _userSummaryMapper = userSummaryMapper;
        }

        #endregion

        #region execution

        public async Task<UserSummary> ExecuteAsync(GetUserSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await Query(query).SingleOrDefaultAsync();
            var user = _userSummaryMapper.Map(dbResult);

            ValidatePermission(query, executionContext, user);

            return user;
        }

        private IQueryable<User> Query(GetUserSummaryByIdQuery query)
        {
            return _dbContext
                .Users
                .AsNoTracking()
                .Include(u => u.Role)
                .Include(u => u.Creator)
                .Where(u => u.UserId == query.UserId);
        }

        private void ValidatePermission(GetUserSummaryByIdQuery query, IExecutionContext executionContext, UserMicroSummary user)
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
