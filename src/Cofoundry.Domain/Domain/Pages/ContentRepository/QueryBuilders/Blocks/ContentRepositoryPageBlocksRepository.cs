using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageBlocksRepository
            : IAdvancedContentRepositoryPageBlocksRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageBlocksRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryPageBlockByIdQueryBuilder GetById(int pageVersionBlockId)
        {
            return new ContentRepositoryPageBlockByIdQueryBuilder(ExtendableContentRepository, pageVersionBlockId);
        }

        #endregion

        #region commands

        public Task AddAsync(AddPageVersionBlockCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateAsync(UpdatePageVersionBlockCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task MoveAsync(MovePageVersionBlockCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int pageVersionBlockId)
        {
            var command = new DeletePageVersionBlockCommand() { PageVersionBlockId = pageVersionBlockId };
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion
    }
}
