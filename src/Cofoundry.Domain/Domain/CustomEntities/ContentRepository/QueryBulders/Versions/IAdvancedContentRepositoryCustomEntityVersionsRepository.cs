using System;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for custom entity versions.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityVersionsRepository
    {
        /// <summary>
        /// Query for paged sets of custom entity version data for a 
        /// specific custom entity.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityVersionsByCustomEntityIdQueryBuilder GetByCustomEntityId();

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
        /// Updates the draft version of a custom entity. If a draft version
        /// does not exist then one is created first from the currently
        /// published version.
        /// </summary>
        /// <param name="customEntityId">
        /// Database id of the custom enitity to update the draft version
        /// for. A custom entity can only have one draft version. If a draft
        /// version does not exist then one is created from the currently
        /// published version.
        /// </param>
        /// <param name="commandPatcher">
        /// An action to configure or "patch" a command that's been initialized
        /// with the existing draft entity data.
        /// </param>
        Task UpdateDraftAsync(int customEntityId, Action<UpdateCustomEntityDraftVersionCommand> commandPatcher);

        /// <summary>
        /// Deletes the draft verison of a custom entity permanently if 
        /// it exists. If no draft exists then no action is taken.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to delete the draft version for.</param>
        Task DeleteDraftAsync(int customEntityId);

        /// <summary>
        /// Queries and commands for regions in page.
        /// </summary>
        IAdvancedContentRepositoryPageRegionsRepository Regions();
    }
}