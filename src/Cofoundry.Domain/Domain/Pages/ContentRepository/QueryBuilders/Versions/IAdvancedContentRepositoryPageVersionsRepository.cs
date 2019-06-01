using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page versions.
    /// </summary>
    public interface IAdvancedContentRepositoryPageVersionsRepository
    {
        #region queries
        
        /// <summary>
        /// Search for page entities, returning paged lists of data.
        /// </summary>
        IAdvancedContentRepositoryPageVersionsByPageIdQueryBuilder GetByPageId();

        /// <summary>
        /// Determines if a page has a draft version of not. A page can only have one draft
        /// version at a time.
        /// </summary>
        /// <param name="pageId">PageId of the page to check.</param>
        Task<bool> HasDraftAsync(int pageId);
               
        #endregion

        #region commands

        /// <summary>
        /// Creates a new draft version of a page from the currently published version. If there
        /// isn't a currently published version then an exception will be thrown. An exception is also 
        /// thrown if there is already a draft version.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task AddDraftAsync(AddPageDraftVersionCommand command);

        /// <summary>
        /// Updates the draft version of a page. If a draft version
        /// does not exist then one is created first.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateDraftAsync(UpdatePageDraftVersionCommand command);

        /// <summary>
        /// Deletes the draft verison of a page permanently if 
        /// it exists. If no draft exists then no action is taken.
        /// </summary>
        /// <param name="pageId">Id of the page to delete the draft version for.</param>
        Task DeleteDraftAsync(int pageId);

        #endregion

        #region child entities

        /// <summary>
        /// Queries and commands for regions in page.
        /// </summary>
        IAdvancedContentRepositoryPageRegionsRepository Regions();

        #endregion
    }
}
