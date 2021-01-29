using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageByDirectoryIdQueryBuilder
        : IContentRepositoryPageByDirectoryIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageDirectoryId;

        public ContentRepositoryPageByDirectoryIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageDirectoryId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageDirectoryId = pageDirectoryId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<ICollection<PageRoute>> AsRoutes()
        {
            var query = new GetPageRoutesByPageDirectoryIdQuery(_pageDirectoryId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
