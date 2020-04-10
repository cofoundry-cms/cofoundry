using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryCustomEntityByPathQueryBuilder
        : IAdvancedContentRepositoryCustomEntityByPathQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityByPathQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<CustomEntityRoute> AsCustomEntityRoute(GetCustomEntityRouteByPathQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
