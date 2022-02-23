using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryUserAccountRecoveryRepository
            : IAdvancedContentRepositoryUserAccountRecoveryRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryUserAccountRecoveryRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<AuthorizedTaskTokenValidationResult> Validate(ValidateUserAccountRecoveryByEmailQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public Task InitiateAsync(InitiateUserAccountRecoveryViaEmailCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task CompleteAsync(CompleteUserAccountRecoveryViaEmailCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}