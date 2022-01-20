using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class GetUpdateCurrentUserCommandByIdQueryHandler
        : IQueryHandler<GetUpdateCommandByIdQuery<UpdateCurrentUserCommand>, UpdateCurrentUserCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IPermissionValidationService _permissionValidationService;

        public GetUpdateCurrentUserCommandByIdQueryHandler(
            CofoundryDbContext dbContext,
            IPermissionValidationService permissionValidationService
            )
        {
            _dbContext = dbContext;
            _permissionValidationService = permissionValidationService;
        }

        public async Task<UpdateCurrentUserCommand> ExecuteAsync(GetUpdateCommandByIdQuery<UpdateCurrentUserCommand> query, IExecutionContext executionContext)
        {
            if (!executionContext.UserContext.UserId.HasValue) return null;

            var user = await _dbContext
                .Users
                .AsNoTracking()
                .FilterCanLogIn()
                .FilterById(executionContext.UserContext.UserId.Value)
                .Select(u => new UpdateCurrentUserCommand()
                {
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName
                })
                .SingleOrDefaultAsync();

            return user;
        }
    }
}
