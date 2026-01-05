using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for pages.
/// </summary>
public static class ContentRepositoryPageExtensions
{
    extension(IContentRepository contentRepository)
    {
        /// <summary>
        /// Pages represent the dynamically navigable pages of your website. Each page uses a template 
        /// which defines the regions of content that users can edit. Pages are a versioned entity and 
        /// therefore have many page version records. At one time a page may only have one draft 
        /// version, but can have many published versions; the latest published version is the one that 
        /// is rendered when the page is published. 
        /// </summary>
        public IContentRepositoryPageRepository Pages()
        {
            return new ContentRepositoryPageRepository(contentRepository.AsExtendableContentRepository());
        }
    }

    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Pages represent the dynamically navigable pages of your website. Each page uses a template 
        /// which defines the regions of content that users can edit. Pages are a versioned entity and 
        /// therefore have many page version records. At one time a page may only have one draft 
        /// version, but can have many published versions; the latest published version is the one that 
        /// is rendered when the page is published. 
        /// </summary>
        public IAdvancedContentRepositoryPageRepository Pages()
        {
            return new AdvancedContentRepositoryPageRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
