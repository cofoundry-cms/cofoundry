using Cofoundry.Domain.Extendable;

namespace Cofoundry.Domain.Internal;

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
