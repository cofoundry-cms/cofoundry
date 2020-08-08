using Cofoundry.Domain.Extendable;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Internal
{
    public class ContentRepositoryCustomEntityPageRegionsRepository
            : IAdvancedContentRepositoryCustomEntityPageRegionsRepository
            , IExtendableContentRepositoryPart
    {
        public ContentRepositoryCustomEntityPageRegionsRepository(
            IExtendableContentRepository contentRepository
            )
        {
            ExtendableContentRepository = contentRepository;
        }

        public IExtendableContentRepository ExtendableContentRepository { get; }

        public IAdvancedContentRepositoryCustomEntityPageBlocksRepository Blocks()
        {
            return new ContentRepositoryCustomEntityPageBlocksRepository(ExtendableContentRepository);
        }
    }
}
