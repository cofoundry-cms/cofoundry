using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class AdvancedContentRepositoryCustomEntityRepository
            : IAdvancedContentRepositoryCustomEntityRepository
            , IExtendableContentRepositoryPart
    {
        public AdvancedContentRepositoryCustomEntityRepository(
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

        public Task<bool> IsPathUniqueAsync(IsPagePathUniqueQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }

        #endregion

        #region commands
        
        public Task AddAsync(AddCustomEntityCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DuplicateAsync(DuplicateCustomEntityCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task PublishAsync(PublishCustomEntityCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UnPublishAsync(UnPublishCustomEntityCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }
        
        public Task UpdateUrlAsync(UpdateCustomEntityUrlCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task ReOrderAsync(ReOrderCustomEntitiesCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task UpdateOrderingPositionAsync(UpdateCustomEntityOrderingPositionCommand command)
        {
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        public Task DeleteAsync(int customEntityId)
        {
            var command = new DeleteCustomEntityCommand()
            {
                CustomEntityId = customEntityId
            };

            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion

        #region child entities

        public IAdvancedContentRepositoryCustomEntityVersionsRepository Versions()
        {
            return new ContentRepositoryCustomEntityVersionsRepository(ExtendableContentRepository);
        }

        /// <summary>
        /// Custom entity definitions are used to define the identity and
        /// behavior of a custom entity type. This includes meta data such
        /// as the name and description, but also the configuration of
        /// features such as whether the identity can contain a locale
        /// and whether versioning (i.e. auto-publish) is enabled.
        /// </summary>
        public IAdvancedContentRepositoryCustomEntityDefinitionsRepository Definitions()
        {
            return new AdvancedContentRepositoryCustomEntityDefinitionsRepository(ExtendableContentRepository);
        }

        /// <summary>
        /// Queries for working with custom entity data model schemas.
        /// </summary>
        public IAdvancedContentRepositoryCustomEntityDataModelSchemasRepository DataModelSchemas()
        {
            return new ContentRepositoryCustomEntityDataModelSchemasRepository(ExtendableContentRepository);
        }
    #endregion
}
}
