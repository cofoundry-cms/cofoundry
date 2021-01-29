using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageRegionByPageVersionIdQueryBuilder
        : IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder
        , IExtendableContentRepositoryPart
    {
        private readonly int _pageVersionId;

        public ContentRepositoryPageRegionByPageVersionIdQueryBuilder(
            IExtendableContentRepository contentRepository,
            int pageVersionId
            )
        {
            ExtendableContentRepository = contentRepository;
            _pageVersionId = pageVersionId;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IDomainRepositoryQueryContext<ICollection<PageRegionDetails>> AsDetails()
        {
            var query = new GetPageRegionDetailsByPageVersionIdQuery(_pageVersionId);
            return DomainRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
