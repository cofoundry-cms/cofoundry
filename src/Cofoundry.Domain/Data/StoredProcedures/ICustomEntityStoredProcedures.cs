using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Data access abstraction over custom entity stored procedures.
    /// </summary>
    public interface ICustomEntityStoredProcedures
    {
        /// <summary>
        /// Adds a draft custom entity version, copying all page blocks and other dependencies. This
        /// query autmatically updates the CustomEntityPublishStatusQuery lookup table.
        /// </summary>
        /// <param name="customEntityId">The id of the custom entity to create the draft for.</param>
        /// <param name="copyFromCustomEntityVersionId">Optional id of the version to copy from, if null we copy from the latest published version.</param>
        /// <param name="createDate">Date to set as the create date for the new version.</param>
        /// <param name="creatorId">Id of the user who created the draft.</param>
        /// <returns>CustomEntityVersionId of the newly created draft version.</returns>
        Task<int> AddDraftAsync(
            int customEntityId,
            int? copyFromCustomEntityVersionId,
            DateTime createDate,
            int creatorId
            );

        /// <summary>
        /// Updates the CustomEntityPublishStatusQuery lookup table for all 
        /// PublishStatusQuery values.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to update.</param>
        Task UpdatePublishStatusQueryLookupAsync(int customEntityId);

        /// <summary>
        /// Re-orders a set of custom entity ids in bulk.
        /// </summary>
        /// <param name="customEntityDefinitionCode">The entity definition code to order. </param>
        /// <param name="orderedCustomEntityIds">
        /// Collection of custom entity ids in the correct order that 
        /// should be applied. Any missing entity ids will be given a null ordering (last).
        /// </param>
        /// <param name="localeId">Locale to set the ordering for. Ordering can only be applied for one locale at a time.</param>
        /// <returns>A collection of ids of custom entities that were updated i.e. some entities may not have needed their ordering changed.</returns>
        Task<ICollection<int>> ReOrderAsync(
           string customEntityDefinitionCode,
           int[] orderedCustomEntityIds,
           int? localeId
           );
    }
}
