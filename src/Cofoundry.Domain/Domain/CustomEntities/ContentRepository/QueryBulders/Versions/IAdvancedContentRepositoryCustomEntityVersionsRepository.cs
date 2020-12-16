using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for custom entity versions.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityVersionsRepository
    {
        #region queries

        /// <summary>
        /// Query for paged sets of custom entity version data for a 
        /// specific custom entity.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder GetByCustomEntityId();

        #endregion

        #region commands

        /// <summary>
        /// Creates a new draft version of a custom entity from the currently published version. If there
        /// isn't a currently published version then an exception will be thrown. An exception is also 
        /// thrown if there is already a draft version.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created draft version.</returns>
        Task<int> AddDraftAsync(AddCustomEntityDraftVersionCommand command);

        /// <summary>
        /// Updates the draft version of a custom entity. If a draft version
        /// does not exist then one is created first from the currently
        /// published version.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateDraftAsync(UpdateCustomEntityDraftVersionCommand command);

        /// <summary>
        /// Deletes the draft verison of a custom entity permanently if 
        /// it exists. If no draft exists then no action is taken.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to delete the draft version for.</param>
        Task DeleteDraftAsync(int customEntityId);

        #endregion

        #region child entities

        /// <summary>
        /// Queries and commands for regions in page.
        /// </summary>
        IAdvancedContentRepositoryPageRegionsRepository Regions();

        #endregion
    }
}
