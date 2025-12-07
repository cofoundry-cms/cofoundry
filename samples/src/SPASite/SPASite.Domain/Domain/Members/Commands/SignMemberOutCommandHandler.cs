namespace SPASite.Domain;

/// <summary>
/// A simple command handler to wrap member signout logic. Although it's 
/// only a one-liner, we've created a handler just to keep it consistent
/// with the reset of the domain logic.
/// </summary>
public class SignMemberOutCommandHandler
    : ICommandHandler<SignMemberOutCommand>
    , IIgnorePermissionCheckHandler
{
    private readonly IAdvancedContentRepository _contentRepository;

    public SignMemberOutCommandHandler(
        IAdvancedContentRepository contentRepository
        )
    {
        _contentRepository = contentRepository;
    }

    public Task ExecuteAsync(SignMemberOutCommand command, IExecutionContext executionContext)
    {
        return _contentRepository
            .Users()
            .Authentication()
            .SignOutAsync();
    }
}
