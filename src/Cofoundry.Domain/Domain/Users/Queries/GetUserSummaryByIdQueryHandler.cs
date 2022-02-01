using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Finds a user by a database id returning a <see cref="UserSummary"/> projection 
    /// if it is found, otherwise <see langword="null"/>.
    /// </summary>
    public class GetUserSummaryByIdQueryHandler
        : IQueryHandler<GetUserSummaryByIdQuery, UserSummary>
        , IIgnorePermissionCheckHandler
    {
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

        public async Task<UserSummary> ExecuteAsync(GetUserSummaryByIdQuery query, IExecutionContext executionContext)
        {
            var dbResult = await _dbContext
                .Users
                .AsNoTracking()
                .IncludeForSummary()
                .FilterById(query.UserId)
                .SingleOrDefaultAsync();
            var user = _userSummaryMapper.Map(dbResult);

            ValidatePermission(query, executionContext, user);

            return user;
        }

        private void ValidatePermission(GetUserSummaryByIdQuery query, IExecutionContext executionContext, UserMicroSummary user)
        {
            if (user == null) return;

            if (user.UserArea.UserAreaCode == CofoundryAdminUserArea.Code)
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<CofoundryUserReadPermission>(query.UserId, executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforceCurrentUserOrHasPermission<NonCofoundryUserReadPermission>(query.UserId, executionContext.UserContext);
            }
        }
    }
}
