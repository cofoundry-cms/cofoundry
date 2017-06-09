using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;

namespace Cofoundry.Domain
{
    public static class CustomEntityVersionQueryExtensions
    {
        /// <summary>
        /// Filters by workflow status query. Note that this may contain a group by clause and you therefore
        /// will need to apply any includes after you have called this statement otherwise they may be ignored.
        /// </summary>
        public static IQueryable<CustomEntityVersion> FilterByWorkFlowStatusQuery(
            this IQueryable<CustomEntityVersion> dbQuery, 
            WorkFlowStatusQuery workFlowStatusQuery,
            int? versionId = null)
        {
            switch (workFlowStatusQuery)
            {
                case WorkFlowStatusQuery.Draft:
                    dbQuery = dbQuery.Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft);
                    break;
                case WorkFlowStatusQuery.Published:
                    dbQuery = dbQuery.Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published);
                    break;
                case WorkFlowStatusQuery.Latest:
                    dbQuery = dbQuery
                        .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                        .GroupBy(e => e.CustomEntityId, (key, g) => g.OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft).FirstOrDefault());
                    break;
                case WorkFlowStatusQuery.PreferPublished:
                    dbQuery = dbQuery
                        .Where(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Draft || v.WorkFlowStatusId == (int)WorkFlowStatus.Published)
                        .GroupBy(e => e.CustomEntityId, (key, g) => g.OrderByDescending(v => v.WorkFlowStatusId == (int)WorkFlowStatus.Published).FirstOrDefault());
                    break;
                case WorkFlowStatusQuery.SpecificVersion:
                    dbQuery = dbQuery.Where(v => v.CustomEntityVersionId == versionId);
                    break;
                default:
                    throw new ArgumentException("Unknown WorkFlowStatusQuery: " + workFlowStatusQuery);
            }

            return dbQuery;
        }
    }
}
