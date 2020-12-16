using Cofoundry.Core;
using Cofoundry.Core.Data;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Data access abstraction over stored procedures for page entities.
    /// </summary>
    public interface IPageStoredProcedures
    {
        /// <summary>
        /// Adds a draft page version, copying all page blocks and other dependencies. This
        /// query autmatically updates the PagePublishStatusQuery lookup table.
        /// </summary>
        /// <param name="pageId">The id of the page to create the draft for.</param>
        /// <param name="copyFromPageVersionId">Optional id of the version to copy from, if null we copy from the latest published version.</param>
        /// <param name="createDate">Date to set as the create date for the new version.</param>
        /// <param name="creatorId">Id of the user who created the draft.</param>
        /// <returns>PageVersionId of the newly created draft version.</returns>
        Task<int> AddDraftAsync(
            int pageId,
            int? copyFromPageVersionId,
            DateTime createDate,
            int creatorId
            );

        /// <summary>
        /// Copies all the blocks from one page version into the draft version of a 
        /// page. The version to copy from does not have to belong to the target page, 
        /// but must use the same page template. The page to copy to should already
        /// have a draft version.
        /// </summary>
        /// <param name="copyToPageId">
        /// Id of the page with a draft to copy the blocks to. The page should already
        /// have a draft version; the procedure will throw an error if a draft version 
        /// is not found.
        /// </param>
        /// <param name="copyFromPageVersionId">
        /// Id of the page version to copy from. The version does not have to belong to
        /// the same page, but must use the same page template otherwise an exception
        /// will be thrown.
        /// </param>        
        /// <param name="createDate">Date to set as the create date for the new blocks.</param>
        /// <param name="creatorId">Id of the user to set as the creator fo the new blocks.</param>
        Task CopyBlocksToDraftAsync(
            int copyToPageId,
            int copyFromPageVersionId,
            DateTime createDate,
            int creatorId
            );

        /// <summary>
        /// Updates the PagePublishStatusQuery lookup table for all 
        /// PublishStatusQuery values.
        /// </summary>
        /// <param name="pageId">Id of the page to update.</param>
        Task UpdatePublishStatusQueryLookupAsync(int pageId);
    }
}
