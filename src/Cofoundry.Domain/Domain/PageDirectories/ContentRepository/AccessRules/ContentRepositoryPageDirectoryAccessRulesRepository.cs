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

        public Task UpdateAsync(UpdatePageDirectoryAccessRuleSetCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}
