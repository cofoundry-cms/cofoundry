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

        public IDomainRepositoryQueryContext<bool> IsUnique(IsPageAccessRuleUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public async Task<int> AddAsync(AddPageAccessRuleCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputPageAccessRuleId;
        }

        public Task UpdateAsync(UpdatePageAccessRuleCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int pageAccessRuleId)
        {
            var command = new DeletePageAccessRuleCommand() { PageAccessRuleId = pageAccessRuleId };
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
    }
}
