using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
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

        public IContentRepositoryQueryContext<ICollection<PageRegionDetails>> AsDetails()
        {
            var query = new GetPageRegionDetailsByPageVersionIdQuery(_pageVersionId);
            return ContentRepositoryQueryContextFactory.Create(query, ExtendableContentRepository);
        }
    }
}
