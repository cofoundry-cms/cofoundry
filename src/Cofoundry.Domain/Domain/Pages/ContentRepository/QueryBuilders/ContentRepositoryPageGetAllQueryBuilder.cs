using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class ContentRepositoryPageGetAllQueryBuilder
        : IContentRepositoryPageGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IContentRepositoryQueryContext<ICollection<PageRoute>> AsRoutes()
        {
            var query = new GetAllPageRoutesQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
