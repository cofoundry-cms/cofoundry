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
        /// Gets version routing info for the specified PublishStatus query
        /// </summary>
        public static T GetVersionRouting<T>(this IEnumerable<T> versions, PublishStatusQuery status, int? versionId = null)
            where T : IVersionRoute
        {
            T result;

            switch (status)
            {
                case PublishStatusQuery.Draft:
                    result = versions
                        .SingleOrDefault(v => v.WorkFlowStatus == WorkFlowStatus.Draft);
                    break;
                case PublishStatusQuery.Published:
                    result = versions
                        .Where(v => v.WorkFlowStatus == WorkFlowStatus.Published)
                        .OrderByDescending(v => v.CreateDate)
                        .FirstOrDefault();
                    break;
                case PublishStatusQuery.Latest:
                    result = versions
                        .Where(v => v.WorkFlowStatus == WorkFlowStatus.Draft || v.WorkFlowStatus == WorkFlowStatus.Published)
                        .OrderByDescending(v => v.WorkFlowStatus == WorkFlowStatus.Draft)
                        .FirstOrDefault();
                    break;
                case PublishStatusQuery.PreferPublished:
                    result = versions
                        .Where(v => v.WorkFlowStatus == WorkFlowStatus.Draft || v.WorkFlowStatus == WorkFlowStatus.Published)
                        .OrderByDescending(v => v.WorkFlowStatus == WorkFlowStatus.Published)
                        .FirstOrDefault();
                    break;
                case PublishStatusQuery.SpecificVersion:
                    if (!versionId.HasValue)
                    {
                        throw new InvalidOperationException("PublishStatusQuery.SpecificVersion requires a specific VersionId");
                    }
                    result = versions
                        .SingleOrDefault(v => v.VersionId == versionId);
                    break;
                default:
                    throw new InvalidOperationException("Unrecognised PublishStatusQuery: " + status);
            }

            return result;
        }
    }
}
