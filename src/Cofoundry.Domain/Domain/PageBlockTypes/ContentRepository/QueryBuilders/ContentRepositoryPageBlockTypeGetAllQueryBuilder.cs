using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageBlockTypeGetAllQueryBuilder
        : IContentRepositoryPageBlockTypeGetAllQueryBuilder
        , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageBlockTypeGetAllQueryBuilder(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<ICollection<PageBlockTypeSummary>> AsSummaries()
        {
            var query = new GetAllPageBlockTypeSummariesQuery();
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
