using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AdvancedContentRepositoryCustomEntityDefinitionsRepository
            : IAdvancedContentRepositoryCustomEntityDefinitionsRepository
            , IExtendableContentRepositoryPart
    {
        public AdvancedContentRepositoryCustomEntityDefinitionsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }
        
        #region queries

        public IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder GetAll()
        {
            return new ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder(ExtendableContentRepository);
        }

        public IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder GetByCode(string customEntityDefinitioncode)
        {
            return new ContentRepositoryCustomEntityDefinitionByCodeQueryBuilder(ExtendableContentRepository, customEntityDefinitioncode);
        }

        public IAdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder GetByDisplayModelType(Type displayModelType)
        {
            return new AdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder(ExtendableContentRepository, displayModelType);
        }

        #endregion

        #region command

        public Task EnsureExistsAsync(string customEntityDefinitionCode)
        {
            var command = new EnsureCustomEntityDefinitionExistsCommand(customEntityDefinitionCode);
            return ExtendableContentRepository.ExecuteCommandAsync(command);
        }

        #endregion
    }
}
