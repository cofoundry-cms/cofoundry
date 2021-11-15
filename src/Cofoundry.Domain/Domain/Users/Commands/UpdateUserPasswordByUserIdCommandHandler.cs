using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class UpdateUserPasswordByUserIdCommandHandler
        : ICommandHandler<UpdateUserPasswordByUserIdCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly IUserContextCache _userContextCache;

        public UpdateUserPasswordByUserIdCommandHandler(
            CofoundryDbContext dbContext,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            ITransactionScopeManager transactionScopeManager,
            IUserContextCache userContextCache
            )
        {
            _dbContext = dbContext;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _transactionScopeManager = transactionScopeManager;
            _userContextCache = userContextCache;
        }

        public async Task ExecuteAsync(UpdateUserPasswordByUserIdCommand command, IExecutionContext executionContext)
        {
            var user = await GetUser(command.UserId);
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            var userArea = _userAreaRepository.GetByCode(user.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);
            _passwordUpdateCommandHelper.ValidatePermissions(userArea, executionContext);

            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);

            await _dbContext.SaveChangesAsync();
            _transactionScopeManager.QueueCompletionTask(_dbContext, () => _userContextCache.Clear(user.UserId));
        }

        private Task<User> GetUser(int userId)
        {
            return _dbContext
                .Users
                .FilterById(userId)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();
        }

    }
}
