using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryUserPasswordResetRequestsRepository
            : IAdvancedContentRepositoryUserPasswordResetRequestsRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryUserPasswordResetRequestsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<PasswordResetRequestAuthenticationResult> Validate(ValidatePasswordResetRequestQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public Task InitiateAsync(InitiateUserPasswordResetRequestCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task CompleteAsync(CompleteUserPasswordResetRequestCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}