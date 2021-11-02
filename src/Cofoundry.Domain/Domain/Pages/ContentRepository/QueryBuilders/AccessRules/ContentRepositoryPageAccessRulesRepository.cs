using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageAccessRulesRepository
            : IAdvancedContentRepositoryPageAccessRulesRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageAccessRulesRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task UpdateAsync(UpdatePageAccessRulesCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}
