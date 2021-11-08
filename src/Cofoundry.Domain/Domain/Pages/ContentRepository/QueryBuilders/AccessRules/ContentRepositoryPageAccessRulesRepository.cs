using Cofoundry.Domain.Extendable;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    /// <inheritdoc/>
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

        public IAdvancedContentRepositoryPageAccessByPageIdQueryBuilder GetByPageId(int pageId)
        {
            return new ContentRepositoryPageAccessByPageIdQueryBuilder(ExtendableContentRepository, pageId);
        }

        public Task UpdateAsync(UpdatePageAccessRuleSetCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}
