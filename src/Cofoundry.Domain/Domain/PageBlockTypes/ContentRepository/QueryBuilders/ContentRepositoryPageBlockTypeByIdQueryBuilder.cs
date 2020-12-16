using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageBlockTypeByIdQueryBuilder
        : IContentRepositoryPageBlockTypeByIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageBlockTypeId;

        public ContentRepositoryPageBlockTypeByIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageBlockTypeId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageBlockTypeId = pageBlockTypeId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<PageBlockTypeSummary> AsSummary()
        {
            var query = new GetPageBlockTypeSummaryByIdQuery(_pageBlockTypeId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }

        public IDomainRepositoryQueryContext<PageBlockTypeDetails> AsDetails()
        {
            var query = new GetPageBlockTypeDetailsByIdQuery(_pageBlockTypeId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
