using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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