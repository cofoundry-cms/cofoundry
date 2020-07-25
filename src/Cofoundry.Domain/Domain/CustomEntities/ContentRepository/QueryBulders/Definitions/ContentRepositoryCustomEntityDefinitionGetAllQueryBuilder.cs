using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder
        : IContentRepositoryCustomEntityDefinitionGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityDefinitionGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<ICollection<CustomEntityDefinitionMicroSummary>> AsMicroSummaries()
        {
            var query = new GetAllCustomEntityDefinitionMicroSummariesQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<ICollection<CustomEntityDefinitionSummary>> AsSummaries()
        {
            var query = new GetAllCustomEntityDefinitionSummariesQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
