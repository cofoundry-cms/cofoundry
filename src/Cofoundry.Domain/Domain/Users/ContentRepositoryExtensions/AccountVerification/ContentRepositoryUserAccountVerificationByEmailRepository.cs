using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryUserAccountVerificationByEmailRepository
            : IAdvancedContentRepositoryUserAccountVerificationByEmailRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryUserAccountVerificationByEmailRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<AuthorizedTaskTokenValidationResult> Validate(ValidateUserAccountVerificationByEmailQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public Task InitiateAsync(InitiateUserAccountVerificationViaEmailCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task CompleteAsync(CompleteUserAccountVerificationViaEmailCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}