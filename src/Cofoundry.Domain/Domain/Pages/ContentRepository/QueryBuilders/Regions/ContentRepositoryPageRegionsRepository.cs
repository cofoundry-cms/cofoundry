using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryPageRegionsRepository
            : IAdvancedContentRepositoryPageRegionsRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryPageRegionsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        /// <summary>
        /// Retrieve regions for a specific verison of a page.
        /// </summary>
        public IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder GetByPageVersionId(int pageVersionId)
        {
            return new ContentRepositoryPageRegionByPageVersionIdQueryBuilder(ExtendableContentRepository, pageVersionId);
        }

        /// <summary>
        /// Queries and commands for page version block data.
        /// </summary>
        public IAdvancedContentRepositoryPageBlocksRepository Blocks()
        {
            return new ContentRepositoryPageBlocksRepository(ExtendableContentRepository);
        }
    }
}
