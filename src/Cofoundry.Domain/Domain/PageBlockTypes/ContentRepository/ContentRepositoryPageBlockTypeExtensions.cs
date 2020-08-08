using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    public static class ContentRepositoryPageBlockTypeExtensions
    {
        /// <summary>
        /// Page block types represent a type of content that can be inserted into a content 
        /// region of a page which could be simple content like 'RawHtml', 'Image' or 
        /// 'PlainText'. Custom and more complex block types can be defined by a 
        /// developer. Block types are typically created when the application
        /// starts up in the auto-update process.
        /// </summary>
        public static IAdvancedContentRepositoryPageBlockTypeRepository PageBlockTypes(this IAdvancedContentRepository contentRepository)
        {
            return new ContentRepositoryPageBlockTypeRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
