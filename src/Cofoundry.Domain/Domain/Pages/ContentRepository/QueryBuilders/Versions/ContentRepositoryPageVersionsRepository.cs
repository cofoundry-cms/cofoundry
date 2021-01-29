using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<bool> HasDraft(int pageId)
        {
            var query = new DoesPageHaveDraftVersionQuery(pageId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
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
