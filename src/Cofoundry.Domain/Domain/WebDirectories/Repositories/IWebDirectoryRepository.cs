using Cofoundry.Domain.CQS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Simple facade over web directory data access queries/commands to them more discoverable.
    /// </summary>
    public interface IWebDirectoryRepository
    {
        #region queries

        /// <summary>
        /// Returns all web directories as WebDirectoryRoute instances. The results of this query are cached.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<IEnumerable<WebDirectoryRoute>> GetAllWebDirectoryRoutesAsync(IExecutionContext executionContext = null);

        /// <summary>
        /// Returns a web directory with the specified id as a WebDirectoryRoute instance.
        /// </summary>
        /// <param name="webDirectoryId">WebDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<WebDirectoryRoute> GetWebDirectoryRouteByIdAsync(int webDirectoryId, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns a web directory with the specified id as a WebDirectoryNode instance.
        /// </summary>
        /// <param name="webDirectoryId">WebDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<WebDirectoryNode> GetWebDirectoryNodeByIdAsync(int webDirectoryId, IExecutionContext executionContext = null);

        /// <summary>
        /// Returns a complete tree of web directory nodes, starting
        /// with the root webdirectory as a single node.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task<WebDirectoryNode> GetWebDirectoryTreeAsync(IExecutionContext executionContext = null);

        /// <summary>
        /// Determines if a webdirectory UrlPath is unique
        /// within its parent directory.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        /// <returns>True if the path is unique; otherwise false.</returns>
        Task<bool> IsRoleTitleUniqueAsync(IsWebDirectoryPathUniqueQuery query, IExecutionContext executionContext = null);

        #endregion

        #region commands

        /// <summary>
        /// Adds a new role to a user area with a specific set of permissions.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task AddWebDirectoryAsync(AddWebDirectoryCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Updates an existing role. Also updates the role permission set.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task UpdateWebDirectoryAsync(UpdateWebDirectoryCommand command, IExecutionContext executionContext = null);

        /// <summary>
        /// Deletes a web directory with the specified database id.
        /// </summary>
        /// <param name="webDirtectoryId">Id of the web directory to delete.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        Task DeleteWebDirectoryAsync(int webDirectoryId, IExecutionContext executionContext = null);

        #endregion 
    }
}
