namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for <see cref="IVersionRoute"/>.
/// </summary>
public static class IVersionRouteExtensions
{
    extension<T>(IEnumerable<T> versions) where T : IVersionRoute
    {
        /// <summary>
        /// Determines if there is a published version
        /// </summary>
        public bool HasPublishedVersion()
        {
            return versions.Any(v => v.WorkFlowStatus == WorkFlowStatus.Published);
        }

        /// <summary>
        /// Gets version routing info for the specified PublishStatus query
        /// </summary>
        public T? GetVersionRouting(PublishStatusQuery status, int? versionId = null)
        {
            T? result;

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
