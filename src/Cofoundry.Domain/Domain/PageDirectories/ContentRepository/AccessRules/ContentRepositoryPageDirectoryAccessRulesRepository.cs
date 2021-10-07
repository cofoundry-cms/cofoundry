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

        public IDomainRepositoryQueryContext<bool> IsUnique(IsPageDirectoryAccessRuleUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public async Task<int> AddAsync(AddPageDirectoryAccessRuleCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputPageDirectoryAccessRuleId;
        }

        public Task UpdateAsync(UpdatePageDirectoryAccessRuleCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int pageDirectoryAccessRuleId)
        {
            var command = new DeletePageDirectoryAccessRuleCommand() { PageDirectoryAccessRuleId = pageDirectoryAccessRuleId };
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}
