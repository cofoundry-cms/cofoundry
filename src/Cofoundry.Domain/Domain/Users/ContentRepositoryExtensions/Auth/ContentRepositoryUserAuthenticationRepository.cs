using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

/// <inheritdoc/>
public class ContentRepositoryUserAuthenticationRepository
        : IAdvancedContentRepositoryUserAuthenticationRepository
        , IExtendableContentRepositoryPart
{
    public ContentRepositoryUserAuthenticationRepository(
        IExtendableContentRepository contentRepository
        )
    {
        ExtendableContentRepository = contentRepository;
    }

    public IExtendableContentRepository ExtendableContentRepository { get; }

    public IDomainRepositoryQueryContext<UserCredentialsAuthenticationResult> AuthenticateCredentials(AuthenticateUserCredentialsQuery query)
    {
        return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
    }

    public Task SignInWithCredentialsAsync(SignInUserWithCredentialsCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task SignInAuthenticatedUserAsync(SignInAuthenticatedUserCommand command)
    {
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task SignOutAsync()
    {
        var command = new SignOutCurrentUserCommand();
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }

    public Task SignOutAllUserAreasAsync()
    {
        var command = new SignOutCurrentUserFromAllUserAreasCommand();
        return ExtendableContentRepository.ExecuteCommandAsync(command);
    }
}
