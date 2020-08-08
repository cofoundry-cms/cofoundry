using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityPageBlocksRepository
            : IAdvancedContentRepositoryCustomEntityPageBlocksRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityPageBlocksRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryCustomEntityPageBlockByIdQueryBuilder GetById(int customEntityVersionPageBlockId)
        {
            return new ContentRepositoryCustomEntityPageBlockByIdQueryBuilder(ExtendableContentRepository, customEntityVersionPageBlockId);
        }

        #endregion

        #region commands

        public async Task<int> AddAsync(AddCustomEntityVersionPageBlockCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.CustomEntityVersionId;
        }

        public Task UpdateAsync(UpdateCustomEntityVersionPageBlockCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task MoveAsync(MoveCustomEntityVersionPageBlockCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int customEntityVersionPageBlockId)
        {
            var command = new DeleteCustomEntityVersionPageBlockCommand() { CustomEntityVersionPageBlockId = customEntityVersionPageBlockId };
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion
    }
}
