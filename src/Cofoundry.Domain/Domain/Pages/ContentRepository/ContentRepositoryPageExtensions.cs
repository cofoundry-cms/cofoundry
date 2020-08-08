using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryPageExtensions
    {
        /// <summary>
        /// Pages represent the dynamically navigable pages of your website. Each page uses a template 
        /// which defines the regions of content that users can edit. Pages are a versioned entity and 
        /// therefore have many page version records. At one time a page may only have one draft 
        /// version, but can have many published versions; the latest published version is the one that 
        /// is rendered when the page is published. 
        /// </summary>
        public static IContentRepositoryPageRepository Pages(this IContentRepository contentRepository)
        {
            return new ContentRepositoryPageRepository(contentRepository.AsExtendableContentRepository());
        }

        /// <summary>
        /// Pages represent the dynamically navigable pages of your website. Each page uses a template 
        /// which defines the regions of content that users can edit. Pages are a versioned entity and 
        /// therefore have many page version records. At one time a page may only have one draft 
        /// version, but can have many published versions; the latest published version is the one that 
        /// is rendered when the page is published. 
        /// </summary>
        public static IAdvancedContentRepositoryPageRepository Pages(this IAdvancedContentRepository contentRepository)
        {
            return new AdvancedContentRepositoryPageRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
