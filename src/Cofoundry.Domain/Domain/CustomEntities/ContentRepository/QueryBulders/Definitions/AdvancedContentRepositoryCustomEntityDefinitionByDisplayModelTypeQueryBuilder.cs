using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class AdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder
        : IAdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly Type _displayModelType;

        public AdvancedContentRepositoryCustomEntityDefinitionByDisplayModelTypeQueryBuilder(
            IExtendableContentRepository contentRepository,
            Type displayModelType
            )
        {
            ExtendableContentRepository = contentRepository;
            _displayModelType = displayModelType;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<CustomEntityDefinitionMicroSummary> AsMicroSummary()
        {
            var query = new GetCustomEntityDefinitionMicroSummaryByDisplayModelTypeQuery()
            {
                DisplayModelType = _displayModelType
            };

            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

    }
}
