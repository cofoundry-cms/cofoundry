using Cofoundry.Core;
using Cofoundry.Core.EntityFramework;
using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data.Internal
{
    /// <summary>
    /// Data access abstraction over custom entity stored procedures.
    /// </summary>
    public class CustomEntityStoredProcedures : ICustomEntityStoredProcedures
    {
        private readonly IEntityFrameworkSqlExecutor _entityFrameworkSqlExecutor;
        private readonly CofoundryDbContext _dbContext;

        public CustomEntityStoredProcedures(
            IEntityFrameworkSqlExecutor entityFrameworkSqlExecutor,
            CofoundryDbContext dbContext
            )
        {
            _entityFrameworkSqlExecutor = entityFrameworkSqlExecutor;
            _dbContext = dbContext;
        }

        /// <summary>
        /// Adds a draft custom entity version, copying all page blocks and other dependencies. This
        /// query autmatically updates the CustomEntityPublishStatusQuery lookup table.
        /// </summary>
        /// <param name="customEntityId">The id of the custom entity to create the draft for.</param>
        /// <param name="copyFromCustomEntityVersionId">Optional id of the version to copy from, if null we copy from the latest published version.</param>
        /// <param name="createDate">Date to set as the create date for the new version.</param>
        /// <param name="creatorId">Id of the user who created the draft.</param>
        /// <returns>CustomEntityVersionId of the newly created draft version.</returns>
        public async Task<int> AddDraftAsync(
            int customEntityId,
            int? copyFromCustomEntityVersionId,
            DateTime createDate,
            int creatorId
            )
        {
            var newVersionId = await _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutputAsync<int?>(_dbContext,
                "Cofoundry.CustomEntity_AddDraft",
                "CustomEntityVersionId",
                 new SqlParameter("CustomEntityId", customEntityId),
                 new SqlParameter("CopyFromCustomEntityVersionId", copyFromCustomEntityVersionId),
                 new SqlParameter("CreateDate", createDate),
                 new SqlParameter("CreatorId", creatorId)
                 );

            if (!newVersionId.HasValue)
            {
                throw new UnexpectedSqlStoredProcedureResultException("Cofoundry.CustomEntity_AddDraft", "No CustomEntityVersionId was returned.");
            }

            return newVersionId.Value;
        }

        /// <summary>
        /// Copies all the page blocks from one custom entity version into the draft 
        /// version of another custom entity. The version must be of the same custom 
        /// entity definition.  The custom entity should already have a draft version.
        /// </summary>
        /// <param name="copyToCustomEntityId">
        /// Id of the custom entity with a draft to copy the blocks to. The custom entity 
        /// should already have a draft version; the procedure will throw an error if a draft 
        /// version is not found.
        /// </param>
        /// <param name="copyFromCustomEntityIdVersionId">
        /// Id of the custom entity version to copy from. The version must be of the same custom 
        /// entity definition (i.e. same custom entity definition code) otherwise an exception 
        /// will be thrown.
        /// </param>
        public Task CopyBlocksToDraftAsync(
            int copyToCustomEntityId,
            int copyFromCustomEntityIdVersionId
            )
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.CustomEntity_CopyBlocksToDraft",
                 new SqlParameter("CustomEntityId", copyToCustomEntityId),
                 new SqlParameter("CopyFromCustomEntityVersionId", copyFromCustomEntityIdVersionId)
                 );
        }

        /// <summary>
        /// Updates the CustomEntityPublishStatusQuery lookup table for all 
        /// PublishStatusQuery values.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to update.</param>
        public Task UpdatePublishStatusQueryLookupAsync(int customEntityId)
        {
            return _entityFrameworkSqlExecutor
                .ExecuteCommandAsync(_dbContext,
                "Cofoundry.CustomEntityPublishStatusQuery_Update",
                 new SqlParameter("CustomEntityId", customEntityId)
                 );
        }

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
        public async Task<ICollection<int>> ReOrderAsync(
           string customEntityDefinitionCode,
           ICollection<int> orderedCustomEntityIds,
           int? localeId
           )
        {
            var updatedIdString = await _entityFrameworkSqlExecutor
                .ExecuteCommandWithOutputAsync<string>(_dbContext,
                    "Cofoundry.CustomEntity_ReOrder",
                    "UpdatedIds",
                    new SqlParameter("CustomEntityDefinitionCode", customEntityDefinitionCode),
                    new SqlParameter("CustomEntityIds", string.Join(",", orderedCustomEntityIds)),
                    CreateNullableIntParameter("LocaleId", localeId)
                    );

            var affectedIds = IntParser.ParseFromDelimitedString(updatedIdString).ToList();

            return affectedIds;
        }

        private SqlParameter CreateNullableIntParameter(string name, int? value)
        {
            var parameter = new SqlParameter(name, System.Data.SqlDbType.Int);
            if (value.HasValue)
            {
                parameter.Value = value.Value;
            }

            return parameter;
        }
    }
}
