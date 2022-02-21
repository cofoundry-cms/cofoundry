using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for page version block data.
    /// </summary>
    public interface IAdvancedContentRepositoryPageBlocksRepository
    {
        /// <summary>
        /// Query for page blocks by an id.
        /// </summary>
        /// <param name="pageVersionBlockId">Database id of the page version block to get.</param>
        IAdvancedContentRepositoryPageBlockByIdQueryBuilder GetById(int pageVersionBlockId);

        /// <summary>
        /// Adds a new block to a template region on a page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created block.</returns>
        Task<int> AddAsync(AddPageVersionBlockCommand command);

        /// <summary>
        /// Updates an existing block within a template region 
        /// of a page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageVersionBlockCommand command);

        /// <summary>
        /// Updates an existing block within a template region 
        /// of a page.
        /// </summary>
        /// <param name="pageVersionBlockId">
        /// Id of the block to update.
        /// </param>
        /// <param name="commandPatcher">
        /// An action to configure or "patch" a command that's been initialized
        /// with the existing page block data.
        /// </param>
        Task UpdateAsync(int pageVersionBlockId, Action<UpdatePageVersionBlockCommand> commandPatcher);

        /// <summary>
        /// Moves a block up or down within a multi-block region 
        /// on a page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task MoveAsync(MovePageVersionBlockCommand command);

        /// <summary>
        /// Deletes a block from a template region on a page.
        /// </summary>
        /// <param name="pageVersionBlockId">Id of the block to delete.</param>
        Task DeleteAsync(int pageVersionBlockId);
    }
}