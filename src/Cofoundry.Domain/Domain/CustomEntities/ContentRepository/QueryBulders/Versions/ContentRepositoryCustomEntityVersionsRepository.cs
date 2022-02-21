using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityVersionsRepository
            : IAdvancedContentRepositoryCustomEntityVersionsRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityVersionsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IAdvancedContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder GetByCustomEntityId()
        {
            return new ContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder(ExtendableContentRepository);
        }

        public async Task<int> AddDraftAsync(AddCustomEntityDraftVersionCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.CustomEntityId;
        }

        public Task UpdateDraftAsync(UpdateCustomEntityDraftVersionCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateDraftAsync(int customEntityId, Action<UpdateCustomEntityDraftVersionCommand> commandPatcher)
        {
            return ExtendableContentRepository.PatchCommandAsync(customEntityId, commandPatcher);
        }

        public Task DeleteDraftAsync(int customEntityId)
        {
            var command = new DeleteCustomEntityDraftVersionCommand() { CustomEntityId = customEntityId };
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public IAdvancedContentRepositoryPageRegionsRepository Regions()
        {
            return new ContentRepositoryPageRegionsRepository(ExtendableContentRepository);
        }
    }
}