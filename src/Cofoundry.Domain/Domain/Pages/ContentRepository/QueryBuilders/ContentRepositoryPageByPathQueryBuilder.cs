using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageByPathQueryBuilder
        : IContentRepositoryPageByPathQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageByPathQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public Task<PageRoutingInfo> AsRoutingInfo(GetPageRoutingInfoByPathQuery query)
        {
            return ExtendableContentRepository.ExecuteQueryAsync(query);
        }
    }
}
