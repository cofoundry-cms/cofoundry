using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving image asset data by a unique database id.
    /// </summary>
    public interface IAdvancedContentRepositoryImageAssetByIdQueryBuilder 
        : IContentRepositoryImageAssetByIdQueryBuilder
    {
        /// <summary>
        /// The ImageAssetFile projection represents the file associated 
        /// with a document asset, including stream access to the file itself.
        /// </summary>
        IDomainRepositoryQueryContext<ImageAssetFile> AsFile();

        /// <summary>
        /// The ImageAssetDetails projection contains full image asset 
        /// information. This is specifically used in the admin panel and 
        /// so contains audit data and tagging information.
        /// </summary>
        IDomainRepositoryQueryContext<ImageAssetDetails> AsDetails();
    }
}
