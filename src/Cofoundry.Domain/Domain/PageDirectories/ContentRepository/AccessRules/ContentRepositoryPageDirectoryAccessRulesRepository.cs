using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageDirectoryAccessRulesRepository
            : IAdvancedContentRepositoryPageDirectoryAccessRulesRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageDirectoryAccessRulesRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task UpdateAsync(UpdatePageDirectoryAccessRulesCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}
