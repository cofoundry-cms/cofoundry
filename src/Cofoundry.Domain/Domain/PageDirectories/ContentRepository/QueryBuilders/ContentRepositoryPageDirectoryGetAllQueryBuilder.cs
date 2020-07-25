using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<ICollection<PageDirectoryRoute>> AsRoutes()
        {
            var query = new GetAllPageDirectoryRoutesQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IContentRepositoryQueryContext<PageDirectoryNode> AsTree()
        {
            var query = new GetPageDirectoryTreeQuery();
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
