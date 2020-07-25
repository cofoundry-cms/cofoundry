using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryRoleSearchQueryBuilder
        : IContentRepositoryRoleSearchQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryRoleSearchQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<PagedQueryResult<RoleMicroSummary>> AsMicroSummaries(SearchRolesQuery query)
        {
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
