using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<PagedQueryResult<CustomEntityVersionSummary>> AsVersionSummaries(GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
