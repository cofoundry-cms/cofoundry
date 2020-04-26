using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageVersionsRepository
            : IAdvancedContentRepositoryPageVersionsRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageVersionsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryPageVersionsByPageIdQueryBuilder GetByPageId()
        {
            return new ContentRepositoryPageVersionsByPageIdQueryBuilder(ExtendableContentRepository);
        }

        public Task<bool> HasDraftAsync(int pageId)
        {
            var query = new DoesPageHaveDraftVersionQuery(pageId);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        #endregion

        #region commands

        public async Task<int> AddDraftAsync(AddPageDraftVersionCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputPageVersionId;
        }

        public Task UpdateDraftAsync(UpdatePageDraftVersionCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteDraftAsync(int pageId)
        {
            var command = new DeletePageDraftVersionCommand() { PageId = pageId };
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion

        #region child entities

        public IAdvancedContentRepositoryPageRegionsRepository Regions()
        {
            return new ContentRepositoryPageRegionsRepository(ExtendableContentRepository);
        }

        #endregion
    }
}
