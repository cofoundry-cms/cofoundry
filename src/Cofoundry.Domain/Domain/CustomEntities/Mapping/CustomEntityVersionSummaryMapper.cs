using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain.Internal
{
    /// <summary>
    /// Simple mapper for mapping to CustomEntityVersionSummary objects.
    /// </summary>
    public class CustomEntityVersionSummaryMapper : ICustomEntityVersionSummaryMapper
    {
        private readonly IAuditDataMapper _auditDataMapper;

        public CustomEntityVersionSummaryMapper(
            IAuditDataMapper auditDataMapper
            )
        {
            _auditDataMapper = auditDataMapper;
        }

        /// <summary>
        /// aps a set of paged EF CustomEntityVersion records for a single custom entity into 
        /// CustomEntityVersionSummary objects.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity that these versions belong to.</param>
        /// <param name="dbResult">Paged result set of records to map.</param>
        public PagedQueryResult<CustomEntityVersionSummary> MapVersions(int customEntityId, PagedQueryResult<CustomEntityVersion> dbResult)
        {
            if (dbResult == null) throw new ArgumentNullException(nameof(dbResult));
            if (customEntityId <= 0) throw new ArgumentOutOfRangeException(nameof(customEntityId));

            // We should only check for the latested published version on the first page
            // as it will only be 1st or 2nd in the list (depending on whether there is a draft)
            bool hasLatestPublishVersion = dbResult.PageNumber > 1;
            var results = new List<CustomEntityVersionSummary>(dbResult.Items.Count);

            foreach (var dbVersion in dbResult.Items.OrderByLatest())
            {
                if (dbVersion.CustomEntityId != customEntityId)
                {
                    var msg = $"Invalid CustomEntityId. CustomEntityVersionSummaryMapper.MapVersions is designed to map versions for a single custom entity only. Excepted CustomEntityId {customEntityId} got {dbVersion.CustomEntityId}";
                    throw new Exception(msg);
                }

                var result = MapVersion(dbVersion);
                if (!hasLatestPublishVersion && result.WorkFlowStatus == WorkFlowStatus.Published)
                {
                    result.IsLatestPublishedVersion = true;
                    hasLatestPublishVersion = true;
                }

                results.Add(result);
            }

            return dbResult.ChangeType(results);
        }

        /// <summary>
        /// Maps an EF CustomEntityVersion record from the db into a CustomEntityVersionSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbVersion">CustomEntityVersion record from the database.</param>
        private CustomEntityVersionSummary MapVersion(CustomEntityVersion dbVersion)
        {
            var versionSummary = new CustomEntityVersionSummary()
            {
                CustomEntityVersionId = dbVersion.CustomEntityVersionId,
                DisplayVersion = dbVersion.DisplayVersion,
                Title = dbVersion.Title,
                WorkFlowStatus = (WorkFlowStatus)dbVersion.WorkFlowStatusId,
                AuditData = _auditDataMapper.MapCreateAuditData(dbVersion)
            };

            return versionSummary;
        }
    }
}
