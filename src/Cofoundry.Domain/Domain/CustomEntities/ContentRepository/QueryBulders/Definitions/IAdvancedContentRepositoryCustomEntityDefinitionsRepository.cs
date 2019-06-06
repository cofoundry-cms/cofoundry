using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries and commands for custom entity definitions.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityDefinitionsRepository
    {
        #region queries

        // TODO
        IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder GetByCode(string customEntityDefinitioncode);
        IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder GetAll(string customEntityDefinitioncode);
        IAdvancedContentRepositoryPageRegionByPageVersionIdQueryBuilder GetDisplayModelType(string customEntityDefinitioncode);

        #endregion

        #region commands

        /// <summary>
        /// CustomEntityDefinitions are definied in code and stored in the database, so if they are missing
        /// from the databse we need to add them. Execute this to ensure that the custom entity definition has been saved
        /// to the database before assigning it to another entity.
        /// </summary>
        /// <param name="customEntityDefinitionCode">
        /// Unique 6 character definition code of the custom entity type
        /// to run the command on.
        /// </param>
        Task EnsureExistsAsync(string customEntityDefinitionCode);

        #endregion
    }
}
