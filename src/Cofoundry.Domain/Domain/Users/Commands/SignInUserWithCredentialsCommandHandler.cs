using Cofoundry.Domain.CQS;
using System.Threading.Tasks;
using Cofoundry.Core.Validation;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Signs a user into the application for a specified user area
    /// using username and password credentials to authenticate. Additional
    /// security checks are also made such as preventing excessive authentication 
    /// attempts. Validation errors are thrown as <see cref="ValidationErrorException"/>. 
    /// The ambient user area (i.e. "current" user context) is switched to the specified area 
    /// for the remainder of the DI scope (i.e. request for web apps).
    /// </summary>
    public class SignInUserWithCredentialsCommandHandler
        : ICommandHandler<SignInUserWithCredentialsCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly IDomainRepository _domainRepository;

        public SignInUserWithCredentialsCommandHandler(
            IDomainRepository domainRepository
            )
        {
            _domainRepository = domainRepository;
        }

        public async Task ExecuteAsync(SignInUserWithCredentialsCommand command, IExecutionContext executionContext)
        {
            var authResult = await _domainRepository
                .WithContext(executionContext)
                .ExecuteQueryAsync(new AuthenticateUserCredentialsQuery()
                {
                    UserAreaCode = command.UserAreaCode,
                    Username = command.Username,
                    Password = command.Password,
                    PropertyToValidate = nameof(command.Password)
                });

            authResult.ThrowIfNotSuccess();

            await _domainRepository.ExecuteCommandAsync(new SignInAuthenticatedUserCommand()
            {
                UserId = authResult.User.UserId,
                RememberUser = command.RememberUser
            });
        }
    }
}