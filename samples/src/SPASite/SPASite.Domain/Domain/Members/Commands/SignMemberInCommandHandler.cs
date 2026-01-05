namespace SPASite.Domain;

/// <summary>
/// Cofoundry has a number of apis to help you validate
/// and sign users in, but here were going to simply wrap
/// the Cofoundry SignInUserWithCredentialsCommand which handles
/// validation, authentication and additional security checks 
/// such as rate limiting sign in attempts.
/// </summary>
public class SignMemberInCommandHandler
    : ICommandHandler<SignMemberInCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly IAdvancedContentRepository _contentRepository;

    public SignMemberInCommandHandler(IAdvancedContentRepository contentRepository)
    {
        _contentRepository = contentRepository;
    }

    public Task ExecuteAsync(SignMemberInCommand command, IExecutionContext executionContext)
    {
        return _contentRepository
            .Users()
            .Authentication()
            .SignInWithCredentialsAsync(new SignInUserWithCredentialsCommand()
            {
                Username = command.Email,
                Password = command.Password,
                UserAreaCode = MemberUserArea.Code,
                RememberUser = true
            });
    }
}
