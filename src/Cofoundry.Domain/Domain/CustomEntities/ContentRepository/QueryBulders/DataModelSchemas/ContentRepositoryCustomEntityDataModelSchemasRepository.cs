using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityDataModelSchemasRepository
            : IAdvancedContentRepositoryCustomEntityDataModelSchemasRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityDataModelSchemasRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        #region queries

        public IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder GetByCustomEntityDefinitionCode(string customEntityDefinitionCode)
        {
            return new ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeQueryBuilder(ExtendableContentRepository, customEntityDefinitionCode);
        }

        public IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder GetByCustomEntityDefinitionCodeRange(IEnumerable<string> customEntityDefinitionCodes)
        {
            return new ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder(ExtendableContentRepository, customEntityDefinitionCodes);
        }

        #endregion
    }
}
