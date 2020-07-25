using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryUserSearchQueryBuilder
        : IContentRepositoryUserSearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryUserSearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<PagedQueryResult<UserSummary>> AsSummaries(SearchUserSummariesQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
