using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<CustomEntityDefinitionMicroSummary> AsMicroSummary()
        {
            var query = new GetCustomEntityDefinitionMicroSummaryByCodeQuery(_customEntityDefinitionCode);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<CustomEntityDefinitionSummary> AsSummary()
        {
            var query = new GetCustomEntityDefinitionSummaryByCodeQuery(_customEntityDefinitionCode);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
