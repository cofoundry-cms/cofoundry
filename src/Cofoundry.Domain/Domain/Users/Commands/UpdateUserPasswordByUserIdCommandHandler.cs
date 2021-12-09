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
        private readonly IPasswordPolicyService _newPasswordValidationService;

        public UpdateUserPasswordByUserIdCommandHandler(
            CofoundryDbContext dbContext,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            ITransactionScopeManager transactionScopeManager,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService
            )
        {
            _dbContext = dbContext;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _transactionScopeManager = transactionScopeManager;
            _userContextCache = userContextCache;
            _newPasswordValidationService = newPasswordValidationService;
        }

        public async Task ExecuteAsync(UpdateUserPasswordByUserIdCommand command, IExecutionContext executionContext)
        {
            var user = await GetUser(command.UserId);
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);
            _passwordUpdateCommandHelper.ValidatePermissions(userArea, executionContext);
            await ValidatePasswordAsync(command, user);
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

        private async Task ValidatePasswordAsync(UpdateUserPasswordByUserIdCommand command, User user)
        {
            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);

            var context = NewPasswordValidationContext.MapFromUser(user);
            context.Password = command.NewPassword;
            context.PropertyName = nameof(command.NewPassword);

            await _newPasswordValidationService.ValidateAsync(context);
        }
    }
}
