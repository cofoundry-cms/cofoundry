using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryUserAccountVerificationRepository
            : IAdvancedContentRepositoryUserAccountVerificationRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryUserAccountVerificationRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task UpdateStatusAsync(UpdateUserAccountVerificationStatusCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public IAdvancedContentRepositoryUserAccountVerificationByEmailRepository EmailFlow()
        {
            return new ContentRepositoryUserAccountVerificationByEmailRepository(ExtendableContentRepository);
        }
    }
}