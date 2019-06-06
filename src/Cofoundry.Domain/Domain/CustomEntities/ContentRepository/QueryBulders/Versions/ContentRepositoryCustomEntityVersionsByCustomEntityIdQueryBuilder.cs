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

        public Task<PagedQueryResult<CustomEntityVersionSummary>> AsVersionSummariesAsync(GetCustomEntityVersionSummariesByCustomEntityIdQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
