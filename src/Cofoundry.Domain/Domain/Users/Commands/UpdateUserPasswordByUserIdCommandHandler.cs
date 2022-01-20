using Cofoundry.Core;
using Cofoundry.Core.MessageAggregator;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.Data.Internal;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class UpdateUserPasswordByUserIdCommandHandler
        : ICommandHandler<UpdateUserPasswordByUserIdCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly CofoundryDbContext _dbContext;
        private readonly IUserStoredProcedures _userStoredProcedures;
        private readonly IDomainRepository _domainRepository;
        private readonly IUserAreaDefinitionRepository _userAreaRepository;
        private readonly IPasswordUpdateCommandHelper _passwordUpdateCommandHelper;
        private readonly IUserSecurityStampUpdateHelper _userSecurityStampUpdateHelper;
        private readonly IUserContextCache _userContextCache;
        private readonly IPasswordPolicyService _newPasswordValidationService;
        private readonly IMessageAggregator _messageAggregator;

        public UpdateUserPasswordByUserIdCommandHandler(
            CofoundryDbContext dbContext,
            IUserStoredProcedures userStoredProcedures,
            IDomainRepository domainRepository,
            IUserAreaDefinitionRepository userAreaRepository,
            IPasswordUpdateCommandHelper passwordUpdateCommandHelper,
            IUserSecurityStampUpdateHelper userSecurityStampUpdateHelper,
            IUserContextCache userContextCache,
            IPasswordPolicyService newPasswordValidationService,
            IMessageAggregator messageAggregator
            )
        {
            _dbContext = dbContext;
            _userStoredProcedures = userStoredProcedures;
            _domainRepository = domainRepository;
            _userAreaRepository = userAreaRepository;
            _passwordUpdateCommandHelper = passwordUpdateCommandHelper;
            _userSecurityStampUpdateHelper = userSecurityStampUpdateHelper;
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

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                await _dbContext.SaveChangesAsync();

                // Typically we only invalidate when the current user changes their password, but we
                // can't be certain of the origin of the request here, so invalidate them anyway.
                await _userStoredProcedures.InvalidateUserAccountRecoveryRequests(user.UserId, executionContext.ExecutionDate);

                scope.QueueCompletionTask(() => OnTransactionComplete(user));
                await scope.CompleteAsync();
            }
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
