using Cofoundry.Core;
using Cofoundry.Core.Data;
using Cofoundry.Core.MessageAggregator;
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
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
        private readonly ITransactionScopeManager _transactionScopeManager;
        private readonly IUserContextCache _userContextCache;
        private readonly IPasswordPolicyService _newPasswordValidationService;
        private readonly IMessageAggregator _messageAggregator;

        public UpdateUserPasswordByUserIdCommandHandler(
            CofoundryDbContext dbContext,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
            ITransactionScopeManager transactionScopeManager,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
            _transactionScopeManager = transactionScopeManager;
            _userContextCache = userContextCache;
            _newPasswordValidationService = newPasswordValidationService;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(UpdateUserPasswordByUserIdCommand command, IExecutionContext executionContext)
        {
            var user = await GetUser(command.UserId);
            EntityNotFoundException.ThrowIfNull(user, command.UserId);

            await ValidatePasswordAsync(command, user, executionContext);
            _passwordUpdateCommandHelper.UpdatePassword(command.NewPassword, user, executionContext);
            _userSecurityStampUpdateHelper.Update(user);

            await _dbContext.SaveChangesAsync();
            await _transactionScopeManager.QueueCompletionTaskAsync(_dbContext, () => OnTransactionComplete(user));
        }

        private async Task OnTransactionComplete(User user)
        {
            _userContextCache.Clear(user.UserId);

            await _userSecurityStampUpdateHelper.OnTransactionCompleteAsync(user);

            await _messageAggregator.PublishAsync(new UserPasswordUpdatedMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId
            });
        }

        private Task<User> GetUser(int userId)
        {
            return _dbContext
                .Users
                .FilterById(userId)
                .FilterCanLogIn()
                .SingleOrDefaultAsync();
        }

        private async Task ValidatePasswordAsync(
            UpdateUserPasswordByUserIdCommand command, 
            User user,
            IExecutionContext executionContext
            )
        {
            var userArea = _userAreaRepository.GetRequiredByCode(user.UserAreaCode);
            _passwordUpdateCommandHelper.ValidateUserArea(userArea);
            _passwordUpdateCommandHelper.ValidatePermissions(userArea, executionContext);

            var context = NewPasswordValidationContext.MapFromUser(user);
            context.Password = command.NewPassword;
            context.PropertyName = nameof(command.NewPassword);
            context.ExecutionContext = executionContext;

            await _newPasswordValidationService.ValidateAsync(context);
        }
    }
}
