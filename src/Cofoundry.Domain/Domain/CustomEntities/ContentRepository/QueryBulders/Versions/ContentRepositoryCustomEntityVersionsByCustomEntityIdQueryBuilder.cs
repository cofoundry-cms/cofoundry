using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder
        : IAdvancedContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<PagedQueryResult<CustomEntityVersionSummary>> AsVersionSummaries(GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
        {
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
