using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
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
        /// Maps an EF CustomEntityVersion record from the db into a CustomEntityVersionSummary 
        /// object. If the db record is null then null is returned.
        /// </summary>
        /// <param name="dbVersion">CustomEntityVersion record from the database.</param>
        public CustomEntityVersionSummary Map(CustomEntityVersion dbVersion)
        {
            if (dbVersion == null) return null;

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
