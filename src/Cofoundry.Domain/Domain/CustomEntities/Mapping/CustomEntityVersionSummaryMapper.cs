using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cofoundry.Domain
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
        /// Maps a collection EF CustomEntityVersion records for a single custom entity into 
        /// a collection of CustomEntityVersionSummary objects.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity that these versions belong to.</param>
        /// <param name="dbVersions">CustomEntityVersion records from the database to map.</param>
        public List<CustomEntityVersionSummary> MapVersions(int customEntityId, ICollection<CustomEntityVersion> dbVersions)
        {
            if (dbVersions == null) throw new ArgumentNullException(nameof(dbVersions));
            if (customEntityId <= 0) throw new ArgumentOutOfRangeException(nameof(customEntityId));

            var orderedVersions = dbVersions
                .Select(MapVersion)
                .OrderByDescending(v => v.WorkFlowStatus == WorkFlowStatus.Draft)
                .ThenByDescending(v => v.AuditData.CreateDate)
                .ToList();

            bool hasLatestPublishVersioned = false;
            var results = new List<CustomEntityVersionSummary>(dbVersions.Count);

            foreach (var dbVersion in dbVersions
                .OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft)
                .ThenByDescending(v => v.CreateDate))
            {
                if (dbVersion.CustomEntityId != customEntityId)
                {
                    var msg = $"Invalid CustomEntityId. CustomEntityVersionSummaryMapper.MapVersions is designed to map versions for a single custom entity only. Excepted CustomEntityId {customEntityId} got {dbVersion.CustomEntityId}";
                    throw new Exception(msg);
                }

                var result = MapVersion(dbVersion);
                if (!hasLatestPublishVersioned && result.WorkFlowStatus == WorkFlowStatus.Published)
                {
                    result.IsLatestPublishedVersion = true;
                    hasLatestPublishVersioned = true;
                }

                results.Add(result);
            }

            return results;
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
                Title = dbVersion.Title,
                WorkFlowStatus = (WorkFlowStatus)dbVersion.WorkFlowStatusId,
                AuditData = _auditDataMapper.MapCreateAuditData(dbVersion)
            };

            return versionSummary;
        }
    }
}
