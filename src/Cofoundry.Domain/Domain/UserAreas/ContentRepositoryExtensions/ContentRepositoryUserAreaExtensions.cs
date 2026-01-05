using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for user areas.
/// </summary>
public static class ContentRepositoryUserAreaExtensions
{
    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Queries and commands relating to user areas and their configuration.
        /// </summary>
        public IAdvancedContentRepositoryUserAreaRepository UserAreas()
        {
            return new ContentRepositoryUserAreaRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
