using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
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

        public IDomainRepositoryQueryContext<UserCredentialsValidationResult> ValidateCredentialsAsync(ValidateUserCredentialsQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public Task SignInWithCredentialsAsync(SignInUserWithCredentialsCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}