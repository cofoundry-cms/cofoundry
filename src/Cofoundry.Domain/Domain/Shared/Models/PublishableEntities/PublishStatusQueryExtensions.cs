namespace Cofoundry.Domain;

/// <summary>
/// Extension methods for <see cref="PublishStatusQuery"/>.
/// </summary>
public static class PublishStatusQueryExtensions
{
    extension(PublishStatusQuery publishStatusQuery)
    {
        /// <summary>
        /// Turns the <see cref="PublishStatusQuery"/> used to query an entity into a
        /// <see cref="PublishStatusQuery"/> that can be used to query dependent entities.
        /// E.g. <see cref="PublishStatusQuery.SpecificVersion"/> cannot be used to query a
        /// dependent entity and so <see cref="PublishStatusQuery.Latest"/> is used instead.
        /// </summary>
        /// <remarks>
        /// When working with child entities, the <see cref="PublishStatusQuery"/> we apply to
        /// them is not neccessarily the status used to query the parent. If we are 
        /// loading a page using the Draft status, then we cannot expect that all 
        /// dependencies should have a draft version, so we re-write it to Latest.
        /// The same applies if we're loading a specific version.
        /// </remarks>
        public PublishStatusQuery ToRelatedEntityQueryStatus()
        {
            if (publishStatusQuery == PublishStatusQuery.Draft || publishStatusQuery == PublishStatusQuery.SpecificVersion)
            {
                publishStatusQuery = PublishStatusQuery.Latest;
            }

            return publishStatusQuery;
        }
    }
}
