using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Completes an account recovery request initiated by
    /// <see cref="InitiateUserAccountRecoveryViaEmailCommand"/>, updating the users
    /// password if the request is verified.
    /// </summary>
    public class CompleteUserAccountVerificationViaEmailCommandHandler
        : ICommandHandler<CompleteUserAccountVerificationViaEmailCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IDomainRepository _domainRepository;
        private readonly IUserContextCache _userContextCache;

        public CompleteUserAccountVerificationViaEmailCommandHandler(
            IDomainRepository domainRepository,
            IUserContextCache userContextCache
            )
        {
            _domainRepository = domainRepository;
            _userContextCache = userContextCache;
        }

        public async Task ExecuteAsync(CompleteUserAccountVerificationViaEmailCommand command, IExecutionContext executionContext)
        {
            var validationResult = await ValidateRequestAsync(command, executionContext);

            using (var scope = _domainRepository.Transactions().CreateScope())
            {
                // authorization is done via the authorization token validation, so we can elevate permissions here
                await _domainRepository
                    .WithElevatedPermissions()
                    .ExecuteCommandAsync(new UpdateUserAccountVerificationStatusCommand()
                    {
                        IsAccountVerified = true,
                        UserId = validationResult.Data.UserId
                    });

                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(new CompleteAuthorizedTaskCommand()
                    {
                        AuthorizedTaskId = validationResult.Data.AuthorizedTaskId
                    });

                await _domainRepository
                    .WithContext(executionContext)
                    .ExecuteCommandAsync(new InvalidateAuthorizedTaskBatchCommand(validationResult.Data.UserId, UserAccountVerificationAuthorizedTaskType.Code));

                scope.QueueCompletionTask(() => OnTransactionComplete(validationResult));
                await scope.CompleteAsync();
            }
        }

        private void OnTransactionComplete(AuthorizedTaskTokenValidationResult validationResult)
        {
            _userContextCache.Clear(validationResult.Data.UserId);
        }

        private async Task<AuthorizedTaskTokenValidationResult> ValidateRequestAsync(
            CompleteUserAccountVerificationViaEmailCommand command,
            IExecutionContext executionContext
            )
        {
            var query = new ValidateUserAccountVerificationByEmailQuery()
            {
                UserAreaCode = command.UserAreaCode,
                Token = command.Token
            };

            var result = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(query);
            result.ThrowIfNotSuccess();

            return result;
        }
    }
}