using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AdvancedContentRepositoryPageDirectoryRepository
            : IAdvancedContentRepositoryPageDirectoryRepository
            , IExtendableContentRepositoryPart
    {
        public AdvancedContentRepositoryPageDirectoryRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryPageDirectoryByIdQueryBuilder GetById(int imageAssetId)
        {
            return new ContentRepositoryPageDirectoryByIdQueryBuilder(ExtendableContentRepository, imageAssetId);
        }

        public IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryPageDirectoryGetAllQueryBuilder(ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsPathUnique(IsPageDirectoryPathUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        #endregion

        #region commands

        public async Task<int> AddAsync(AddPageDirectoryCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputPageDirectoryId;
        }

        public Task UpdateAsync(UpdatePageDirectoryCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int PageDirectoryId)
        {
            var command = new DeletePageDirectoryCommand()
            {
                PageDirectoryId = PageDirectoryId
            };

            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion
    }
}
