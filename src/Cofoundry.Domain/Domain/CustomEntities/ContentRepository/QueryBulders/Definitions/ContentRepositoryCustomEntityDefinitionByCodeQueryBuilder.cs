using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityDefinitionByCodeQueryBuilder
        : IContentRepositoryCustomEntityDefinitionByCodeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly string _customEntityDefinitionCode;

        public ContentRepositoryCustomEntityDefinitionByCodeQueryBuilder(
            IExtendableContentRepository contentRepository,
            string customEntityDefinitionCode
            )
        {
            ExtendableContentRepository = contentRepository;
            _customEntityDefinitionCode = customEntityDefinitionCode;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<CustomEntityDefinitionMicroSummary> AsMicroSummary()
        {
            var query = new GetCustomEntityDefinitionMicroSummaryByCodeQuery(_customEntityDefinitionCode);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<CustomEntityDefinitionSummary> AsSummary()
        {
            var query = new GetCustomEntityDefinitionSummaryByCodeQuery(_customEntityDefinitionCode);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
