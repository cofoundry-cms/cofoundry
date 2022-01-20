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

        public IDomainRepositoryQueryContext<ValidationQueryResult> Validate(ValidateUserAccountRecoveryRequestQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public Task InitiateAsync(InitiateUserAccountRecoveryCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task CompleteAsync(CompleteUserAccountRecoveryCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}