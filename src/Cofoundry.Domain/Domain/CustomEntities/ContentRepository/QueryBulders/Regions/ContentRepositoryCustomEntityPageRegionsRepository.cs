using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

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
