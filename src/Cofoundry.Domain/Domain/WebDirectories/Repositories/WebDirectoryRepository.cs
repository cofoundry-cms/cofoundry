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
    public class WebDirectoryRepository : IWebDirectoryRepository
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;

        public WebDirectoryRepository(
            IQueryExecutor queryExecutor,
            ICommandExecutor commandExecutor
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
        }

        #endregion

        #region queries

        /// <summary>
        /// Returns all web directories as WebDirectoryRoute objects. The results of this query are cached.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public IEnumerable<WebDirectoryRoute> GetAllWebDirectoryRoutes(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAll<WebDirectoryRoute>(executionContext);
        }

        /// <summary>
        /// Returns all web directories as WebDirectoryRoute instances. The results of this query are cached.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<IEnumerable<WebDirectoryRoute>> GetAllWebDirectoryRoutesAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetAllAsync<WebDirectoryRoute>(executionContext);
        }

        /// <summary>
        /// Returns a web directory with the specified id as a WebDirectoryRoute instance.
        /// </summary>
        /// <param name="webDirectoryId">WebDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public WebDirectoryRoute GetWebDirectoryRouteById(int webDirectoryId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<WebDirectoryRoute>(webDirectoryId, executionContext);
        }

        /// <summary>
        /// Returns a web directory with the specified id as a WebDirectoryRoute instance.
        /// </summary>
        /// <param name="webDirectoryId">WebDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<WebDirectoryRoute> GetWebDirectoryRouteByIdAsync(int webDirectoryId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<WebDirectoryRoute>(webDirectoryId, executionContext);
        }

        /// <summary>
        /// Returns a web directory with the specified id as a WebDirectoryNode instance.
        /// </summary>
        /// <param name="webDirectoryId">WebDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public WebDirectoryNode GetWebDirectoryNodeById(int webDirectoryId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetById<WebDirectoryNode>(webDirectoryId, executionContext);
        }

        /// <summary>
        /// Returns a web directory with the specified id as a WebDirectoryNode instance.
        /// </summary>
        /// <param name="webDirectoryId">WebDirectoryId of the directory to get.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<WebDirectoryNode> GetWebDirectoryNodeByIdAsync(int webDirectoryId, IExecutionContext executionContext = null)
        {
            return _queryExecutor.GetByIdAsync<WebDirectoryNode>(webDirectoryId, executionContext);
        }

        /// <summary>
        /// Returns a complete tree of web directory nodes, starting
        /// with the root webdirectory as a single node.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public WebDirectoryNode GetWebDirectoryTreeId(IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(new GetWebDirectoryTreeQuery(), executionContext);
        }

        /// <summary>
        /// Returns a complete tree of web directory nodes, starting
        /// with the root webdirectory as a single node.
        /// </summary>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task<WebDirectoryNode> GetWebDirectoryTreeAsync(IExecutionContext executionContext = null)
        {
            return _queryExecutor.ExecuteAsync(new GetWebDirectoryTreeQuery(), executionContext);
        }

        /// <summary>
        /// Determines if a webdirectory UrlPath is unique
        /// within its parent directory.
        /// </summary>
        /// <param name="query">The parameters run the query with.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        /// <returns>True if the path is unique; otherwise false.</returns>
        public bool IsRoleTitleUnique(IsWebDirectoryPathUniqueQuery query, IExecutionContext executionContext = null)
        {
            return _queryExecutor.Execute(query, executionContext);
        }

        #endregion

        #region commands

        /// <summary>
        /// Adds a new role to a user area with a specific set of permissions.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task AddWebDirectoryAsync(AddWebDirectoryCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// Updates an existing role. Also updates the role permission set.
        /// </summary>
        /// <param name="command">Command to run.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task UpdateWebDirectoryAsync(UpdateWebDirectoryCommand command, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(command, executionContext);
        }

        /// <summary>
        /// Deletes a web directory with the specified database id.
        /// </summary>
        /// <param name="webDirtectoryId">Id of the web directory to delete.</param>
        /// <param name="executionContext">Optional execution context to use when executing the query. Useful if you need to temporarily elevate your permission level.</param>
        public Task DeleteWebDirectoryAsync(int webDirectoryId, IExecutionContext executionContext = null)
        {
            return _commandExecutor.ExecuteAsync(new DeleteWebDirectoryCommand(webDirectoryId), executionContext);
        }

        #endregion 
    }
}
