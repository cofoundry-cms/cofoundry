using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public static class IVersionRouteExtensions
    {
        /// <summary>
        /// Determines if there is a published version
        /// </summary>
        public static bool HasPublishedVersion<T>(this IEnumerable<T> versions)
            where T : IVersionRoute
        {
            return versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Published);
        }

        /// <summary>
        /// Gets version routing info for the specified WorkFlowStatus query
        /// </summary>
        public static T GetVersionRouting<T>(this IEnumerable<T> versions, WorkFlowStatusQuery status, int? versionId = null)
            where T : IVersionRoute
        {
            T result;

            switch (status)
            {
                case WorkFlowStatusQuery.Draft:
                    result = versions
                        .SingleOrDefault(v => v.WorkFlowStatus == WorkFlowStatus.Draft);
                    break;
                case WorkFlowStatusQuery.Published:
                    result = versions
                        .SingleOrDefault(v => v.WorkFlowStatus == WorkFlowStatus.Published);
                    break;
                case WorkFlowStatusQuery.Latest:
                    result = versions
                        .Where(v => v.WorkFlowStatus == WorkFlowStatus.Draft || v.WorkFlowStatus == WorkFlowStatus.Published)
                        .OrderByDescending(v => v.WorkFlowStatus == WorkFlowStatus.Draft)
                        .FirstOrDefault();
                    break;
                case WorkFlowStatusQuery.PreferPublished:
                    result = versions
                        .Where(v => v.WorkFlowStatus == WorkFlowStatus.Draft || v.WorkFlowStatus == WorkFlowStatus.Published)
                        .OrderByDescending(v => v.WorkFlowStatus == WorkFlowStatus.Published)
                        .FirstOrDefault();
                    break;
                case WorkFlowStatusQuery.SpecificVersion:
                    if (!versionId.HasValue)
                    {
                        throw new InvalidOperationException("WorkFlowStatusQuery.SpecificVersion requires a specific VersionId");
                    }
                    result = versions
                        .SingleOrDefault(v => v.VersionId == versionId);
                    break;
                default:
                    throw new InvalidOperationException("Unrecognised WorkFlowStatusQuery: "  + status);
            }

            return result;
        }
    }
}
