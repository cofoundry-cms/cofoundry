using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

public static class ContentRepositoryUserAreaExtensions
{
    /// <summary>
    /// Queries and commands relating to user areas and their configuration.
    /// </summary>
    public static IAdvancedContentRepositoryUserAreaRepository UserAreas(this IAdvancedContentRepository contentRepository)
    {
        return new ContentRepositoryUserAreaRepository(contentRepository.AsExtendableContentRepository());
    }
}
