using Cofoundry.Domain;
using Cofoundry.Domain.CQS;
using System.Threading.Tasks;

namespace Cofoundry.Samples.SPASite.Domain
{
    /// <summary>
    /// Cofoundry has a number of apis to help you validate
    /// and log users in, but here were going to simply wrap
    /// the Cofoundry LogUserInWithCredentialsCommand which handles
    /// validation, authentication and additional security checks 
    /// such as preventing excessive login attempts.
    /// </summary>
    public class SignInMemberCommandHandler
        : ICommandHandler<SignInMemberCommand>
        , IIgnorePermissionCheckHandler
    {
        private readonly ICommandExecutor _commandExecutor;

        public SignInMemberCommandHandler(
            ICommandExecutor commandExecutor
            )
        {
            _commandExecutor = commandExecutor;
        }

        public Task ExecuteAsync(SignInMemberCommand command, IExecutionContext executionContext)
        {
            var logUserInCommand = new SignInUserWithCredentialsCommand()
            {
                Username = command.Email,
                Password = command.Password,
                UserAreaCode = MemberUserArea.Code,
                RememberUser = true
            };

            return _commandExecutor.ExecuteAsync(logUserInCommand);
        }
    }
}