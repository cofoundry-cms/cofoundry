using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Sets the account verification status of the user. Account 
    /// verification is a generic flag to mark a user as verified
    /// or activated. This command isn't concerned with how the
    /// verification has happened, but this is often done via an
    /// email notification or another out-of-band communication
    /// with a verification code.
    /// </summary>
    public class UpdateUserAccountVerificationStatusCommandHandler
        : ICommandHandler<UpdateUserAccountVerificationStatusCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaDefinitionRepository;
        private readonly IPermissionValidationService _permissionValidationService;
        private readonly IUserContextCache _userContextCache;
        private readonly IMessageAggregator _messageAggregator;

        public UpdateUserAccountVerificationStatusCommandHandler(
            CofoundryDbContext dbContext,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaDefinitionRepository,
            IPermissionValidationService permissionValidationService,
            IUserContextCache userContextCache,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _domainRepository = domainRepository;
            _userAreaDefinitionRepository = userAreaDefinitionRepository;
            _permissionValidationService = permissionValidationService;
            _userContextCache = userContextCache;
            _messageAggregator = messageAggregator;
        }

        public async Task ExecuteAsync(UpdateUserAccountVerificationStatusCommand command, IExecutionContext executionContext)
        {
            var user = await GetUserAsync(command.UserId);

            var userArea = _userAreaDefinitionRepository.GetRequiredByCode(user.UserAreaCode);
            ValidatePermissions(userArea, executionContext);
            var hasVerificationStatusChanged = UpdateAccountVerifiedStatus(command, user, executionContext);

            await _dbContext.SaveChangesAsync();

            if (hasVerificationStatusChanged)
            {
                await _domainRepository.Transactions().QueueCompletionTaskAsync(() => OnTransactionComplete(user, command));
            }
        }

        private async Task OnTransactionComplete(User user, UpdateUserAccountVerificationStatusCommand command)
        {
            _userContextCache.Clear();

            await _messageAggregator.PublishAsync(new UserAccountVerificationStatusUpdatedMessage()
            {
                UserAreaCode = user.UserAreaCode,
                UserId = user.UserId,
                IsVerified = command.IsAccountVerified
            });
        }

        private Task<User> GetUserAsync(int userId)
        {
            var user = _dbContext
                .Users
                .FilterById(userId)
                .FilterCanSignIn()
                .SingleOrDefaultAsync();

            EntityNotFoundException.ThrowIfNull(user, userId);

            return user;
        }

        private static bool UpdateAccountVerifiedStatus(UpdateUserAccountVerificationStatusCommand command, User user, IExecutionContext executionContext)
        {
            var hasChanged = user.AccountVerifiedDate.HasValue != command.IsAccountVerified;

            if (command.IsAccountVerified)
            {
                user.AccountVerifiedDate = executionContext.ExecutionDate;
            }
            else
            {
                user.AccountVerifiedDate = null;
            }

            return hasChanged;
        }

        public void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext)
        {
            if (userArea is CofoundryAdminUserArea)
            {
                _permissionValidationService.EnforcePermission(new CofoundryUserUpdatePermission(), executionContext.UserContext);
            }
            else
            {
                _permissionValidationService.EnforcePermission(new NonCofoundryUserUpdatePermission(), executionContext.UserContext);
            }
        }
    }
}
