using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over page directory data access queries/commands to them more discoverable.
    /// </summary>
    [Obsolete("Use the new IContentRepository instead.")]
    public interface IPageDirectoryRepository
    {
        #region queries

        /// <summary>
        /// Returns all page directories as PageDirectoryRoute instances. The results of this query are cached.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<ICollection<PageDirectoryRoute>> GetAllPageDirectoryRoutesAsync(IExecutionContext executionContext = null);

        /// <summary>
        /// Returns a page directory with the specified id as a PageDirectoryRoute instance.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageDirectoryRoute> GetPageDirectoryRouteByIdAsync(int pageDirectoryId, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns a page directory with the specified id as a PageDirectoryNode instance.
        /// </summary>
        /// <param name="pageDirectoryId">PageDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageDirectoryNode> GetPageDirectoryNodeByIdAsync(int pageDirectoryId, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns a complete tree of page directory nodes, starting
        /// with the root page directory as a single node.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<PageDirectoryNode> GetPageDirectoryTreeAsync(IExecutionContext executionContext = null);

        /// <summary>
        /// Determines if a page directory UrlPath is unique
        /// within its parent directory.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        /// <returns>True if the path is unique; otherwise false.</returns>
        Task<bool> IsDirectoryPathUniqueAsync(IsPageDirectoryPathUniqueQuery query, IExecutionContext executionContext = null);

        #endregion

        #region commands

        /// <summary>
        /// Adds a new page directory.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task AddPageDirectoryAsync(AddPageDirectoryCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Updates an existing page directory.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task UpdatePageDirectoryAsync(UpdatePageDirectoryCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Deletes a page directory with the specified database id.
        /// </summary>
        /// <param name="pageDirectoryId">Id of the page directory to delete.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task DeletePageDirectoryAsync(int pageDirectoryId, IExecutionContext executionContext = null);

        #endregion 
    }
}
