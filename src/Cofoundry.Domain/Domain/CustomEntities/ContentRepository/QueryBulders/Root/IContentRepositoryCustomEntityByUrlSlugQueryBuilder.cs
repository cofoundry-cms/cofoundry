using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Queries for retrieving custom entity data using its UrlSlug property.
    /// The UrlSlug property may or may not be unique depending on the settings in the
    /// custom entity definition.
    /// </summary>
    public interface IContentRepositoryCustomEntityByUrlSlugQueryBuilder
    {
        /// <summary>
        /// <para>
        /// Queries a custom entity by its UrlSlug property, returning only a single 
        /// matching result. To use this query the custom entity definition have the
        /// ForceUrlSlugUniqueness property set to true. 
        /// </para>
        /// <para>
        /// This query returns a general-purpose CustomEntityRenderSummary projection 
        /// which includes version specific data and a deserialized data model. 
        /// The result is version-sensitive, use the publishStatusQuery parameter
        /// to control the version returned.
        /// </para>
        /// </summary>
        /// <param name="publishStatusQuery">Used to determine which version of the custom entity to include data for.</param>
        IDomainRepositoryQueryMutator<ICollection<CustomEntityRenderSummary>, CustomEntityRenderSummary> AsRenderSummary(PublishStatusQuery publishStatusQuery);

        /// <summary>
        /// <para>
        /// Queries a custom entity by its UrlSlug property, returning only a single 
        /// matching result. To use this query the custom entity definition have the
        /// ForceUrlSlugUniqueness property set to true. 
        /// </para>
        /// <para>
        /// This query returns a general-purpose CustomEntityRenderSummary projection 
        /// which includes version specific data and a deserialized data model. 
        /// The result is version-sensitive and defaults to returning published 
        /// versions only, but you can use the overload with the publishStatusQuery 
        /// parameter to control this behavior.
        /// </para>
        /// </summary>
        IDomainRepositoryQueryMutator<ICollection<CustomEntityRenderSummary>, CustomEntityRenderSummary> AsRenderSummary();

        /// <summary>
        /// <para>
        /// Queries a custom entity by its UrlSlug property, returning any matches. 
        /// This version of the query should be used when the custom entity definition 
        /// has the ForceUrlSlugUniqueness property set to false. 
        /// </para>
        /// <para>
        /// This query returns a general-purpose CustomEntityRenderSummary projection 
        /// which includes version specific data and a deserialized data model. 
        /// The result is version-sensitive, use the publishStatusQuery parameter
        /// to control the version returned.
        /// </para>
        /// </summary>
        /// <param name="publishStatusQuery">Used to determine which version of the custom entity to include data for.</param>
        IDomainRepositoryQueryContext<ICollection<CustomEntityRenderSummary>> AsRenderSummaries(PublishStatusQuery publishStatusQuery);

        /// <summary>
        /// <para>
        /// Queries a custom entity by its UrlSlug property, returning any matches. 
        /// This version of the query should be used when the custom entity definition 
        /// has the ForceUrlSlugUniqueness property set to false. 
        /// </para>
        /// <para>
        /// This query returns a general-purpose CustomEntityRenderSummary projection 
        /// which includes version specific data and a deserialized data model. 
        /// The result is version-sensitive and defaults to returning published 
        /// versions only, but you can use the overload with the publishStatusQuery 
        /// parameter to control this behavior.
        /// </para>
        /// </summary>
        IDomainRepositoryQueryContext<ICollection<CustomEntityRenderSummary>> AsRenderSummaries();
    }
}
