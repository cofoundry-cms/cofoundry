using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the PageDirectory entity.
    /// </summary>
    public interface IAdvancedContentRepositoryPageDirectoryRepository
    {
        #region queries

        /// <summary>
        /// Retrieve an image asset by a unique database id.
        /// </summary>
        /// <param name="pageDirectoryId">Id of the directory to get.</param>
        IAdvancedContentRepositoryPageDirectoryByIdQueryBuilder GetById(int pageDirectoryId);

        /// <summary>
        /// Queries that return all page directories.
        /// </summary>
        IAdvancedContentRepositoryPageDirectoryGetAllQueryBuilder GetAll();

        /// <summary>
        /// Query to determine if a page directory UrlPath is unique
        /// within its parent directory.
        /// </summary>
        IDomainRepositoryQueryContext<bool> IsPathUnique(IsPageDirectoryPathUniqueQuery query);

        #endregion

        #region commands

        /// <summary>
        /// Adds a new page directory.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created directory.</returns>
        Task<int> AddAsync(AddPageDirectoryCommand command);

        /// <summary>
        /// Updates the properties of an existing page directory, including
        /// properties to configure the directory path an hierarchy.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageDirectoryCommand command);

        /// <summary>
        /// Removes a page directory from the system. The root directory cannot
        /// be deleted.
        /// </summary>
        /// <param name="pageDirectoryId">Id of the page directory to delete.</param>
        Task DeleteAsync(int pageDirectoryId);

        #endregion
    }
}
