using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AdvancedContentRepositoryPageRepository
            : IAdvancedContentRepositoryPageRepository
            , IExtendableContentRepositoryPart
    {
        public AdvancedContentRepositoryPageRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IContentRepositoryPageGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryPageGetAllQueryBuilder(ExtendableContentRepository);
        }

        public IAdvancedContentRepositoryPageByIdQueryBuilder GetById(int pageId)
        {
            return new ContentRepositoryPageByIdQueryBuilder(ExtendableContentRepository, pageId);
        }

        public IAdvancedContentRepositoryPageByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> pageIds)
        {
            return new ContentRepositoryPageByIdRangeQueryBuilder(ExtendableContentRepository, pageIds);
        }

        public IAdvancedContentRepositoryPageSearchQueryBuilder Search()
        {
            return new ContentRepositoryPageSearchQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryPageByPathQueryBuilder GetByPath()
        {
            return new ContentRepositoryPageByPathQueryBuilder(ExtendableContentRepository);
        }

        public IAdvancedContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder GetByCustomEntityDefinitionCode(string customEntityDefinitionCode)
        {
            return new ContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder(ExtendableContentRepository, customEntityDefinitionCode);
        }

        public IAdvancedContentRepositoryPageByCustomEntityIdQueryBuilder GetByCustomEntityId(int customEntityId)
        {
            return new ContentRepositoryPageByCustomEntityIdQueryBuilder(ExtendableContentRepository, customEntityId);
        }

        public IAdvancedContentRepositoryPageByCustomEntityIdRangeQueryBuilder GetByCustomEntityIdRange(IEnumerable<int> customEntityIds)
        {
            return new ContentRepositoryPageByCustomEntityIdRangeQueryBuilder(ExtendableContentRepository, customEntityIds);
        }

        public IContentRepositoryPageByDirectoryIdQueryBuilder GetByDirectoryId(int customEntityId)
        {
            return new ContentRepositoryPageByDirectoryIdQueryBuilder(ExtendableContentRepository, customEntityId);
        }

        public IAdvancedContentRepositoryPageNotFoundQueryBuilder NotFound()
        {
            return new ContentRepositoryPageNotFoundQueryBuilder(ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<bool> IsPathUnique(IsPagePathUniqueQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        #endregion

        #region commands
        
        public async Task<int> AddAsync(AddPageCommand command)
        {
            await ExtendableContentRepository.ExecuteCommandAsync(command);
            return command.OutputPageId;
        }

        public Task DuplicateAsync(DuplicatePageCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task PublishAsync(PublishPageCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UnPublishAsync(UnPublishPageCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateAsync(UpdatePageCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateUrlAsync(UpdatePageUrlCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int pageId)
        {
            var command = new DeletePageCommand()
            {
                PageId = pageId
            };

            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion

        #region child entities

        public IAdvancedContentRepositoryPageVersionsRepository Versions()
        {
            return new ContentRepositoryPageVersionsRepository(ExtendableContentRepository);
        }

        #endregion
    }
}
