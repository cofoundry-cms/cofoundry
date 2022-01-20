using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
    public class ContentRepositoryCurrentUserRepository
            : IAdvancedContentRepositoryCurrentUserRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCurrentUserRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task UpdateAsync(UpdateCurrentUserCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdatePasswordAsync(UpdateCurrentUserPasswordCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public IContentRepositoryCurrentUserQueryBuilder Get()
        {
            return new ContentRepositoryCurrentUserQueryBuilder(ExtendableContentRepository);
        }
    }
}