using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageDirectoryGetAllQueryBuilder
        : IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageDirectoryGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<ICollection<PageDirectoryRoute>> AsRoutes()
        {
            var query = new GetAllPageDirectoryRoutesQuery();
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageDirectoryNode> AsTree()
        {
            var query = new GetPageDirectoryTreeQuery();
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
