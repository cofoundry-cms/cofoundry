using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder
        : IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private string _customEntityDefinitionCode;

        public ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder(
            IExtendableContentRepository contentRepository,
            string customEntityDefinitionCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityDefinitionCode = customEntityDefinitionCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<CustomEntityDataModelSchema> AsDetails()
        {
            var query = new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeQuery(_customEntityDefinitionCode);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
