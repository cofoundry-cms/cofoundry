using Cofoundry.Domain.Extendable;
using Cofoundry.Domain.Internal;

namespace Cofoundry.Domain;

/// <summary>
/// Content repository extension methods for page content blocks.
/// </summary>
public static class ContentRepositoryPageBlockTypeExtensions
{
    extension(IAdvancedContentRepository contentRepository)
    {
        /// <summary>
        /// Page block types represent a type of content that can be inserted into a content 
        /// region of a page which could be simple content like 'RawHtml', 'Image' or 
        /// 'PlainText'. Custom and more complex block types can be defined by a 
        /// developer. Block types are typically created when the application
        /// starts up in the auto-update process.
        /// </summary>
        public IAdvancedContentRepositoryPageBlockTypeRepository PageBlockTypes()
        {
            return new ContentRepositoryPageBlockTypeRepository(contentRepository.AsExtendableContentRepository());
        }
    }
}
