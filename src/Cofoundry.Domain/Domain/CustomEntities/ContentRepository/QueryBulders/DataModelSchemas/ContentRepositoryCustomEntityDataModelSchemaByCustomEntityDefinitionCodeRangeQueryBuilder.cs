using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder
        : IAdvancedContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private IEnumerable<string> _customEntityDefinitionCodes;

        public ContentRepositoryCustomEntityDataModelSchemaByCustomEntityDefinitionCodeRangeQueryBuilder(
            IExtendableContentRepository contentRepository,
            IEnumerable<string> customEntityDefinitionCodes
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityDefinitionCodes = customEntityDefinitionCodes;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<IDictionary<string, CustomEntityDataModelSchema>> AsDetailsAsync()
        {
            var query = new GetCustomEntityDataModelSchemaDetailsByDefinitionCodeRangeQuery(_customEntityDefinitionCodes);
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
