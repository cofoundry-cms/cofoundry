using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
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

        public IDomainRepositoryQueryContext<ICollection<PageRoute>> AsRoutes()
        {
            var query = new GetAllPageRoutesQuery();
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
