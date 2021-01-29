using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the custom entities.
    /// </summary>
    public interface IContentRepositoryCustomEntityRepository
    {
        #region queries

        /// <summary>
        /// Retrieve all custom entities of a type in one query.
        /// </summary>
        /// <param name="customEntityDefinitionCode">
        /// The code identifier for the custom entity type
        /// to query for.
        /// </param>
        IContentRepositoryCustomEntityByDefinitionQueryBuilder GetByDefinitionCode(string customEntityDefinitionCode);

        /// <summary>
        /// Retrieve all custom entities of a type in one query.
        /// </summary>
        /// <typeparam name="TDefinition">The definition type to fetch custom entities for.</typeparam>
        IContentRepositoryCustomEntityByDefinitionQueryBuilder GetByDefinition<TDefinition>() where TDefinition : ICustomEntityDefinition;

        /// <summary>
        /// Retrieve a custom entity by its unique database id.
        /// </summary>
        /// <param name="customEntityId">Id of the custom entity to get.</param>
        IContentRepositoryCustomEntityByIdQueryBuilder GetById(int customEntityId);

        /// <summary>
        /// Retrieve a set of custom entities using a batch of ids.
        /// The Cofoundry.Core dictionary extensions can be useful for 
        /// ordering the results e.g. results.FilterAndOrderByKeys(ids).
        /// </summary>
        /// <param name="customEntityIds">Range of CustomEntityIds of the custom entities to get.</param>
        IContentRepositoryCustomEntityByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> customEntityIds);

        /// <summary>
        /// Fetch custom entities filtering on the UrlSlug property.
        /// </summary>
        /// <param name="customEntityDefinitionCode">
        /// The code identifier for the custom entity type to query for.
        /// </param>
        /// <param name="urlSlug">UrlSlug to filter the results on.</param>
        IContentRepositoryCustomEntityByUrlSlugQueryBuilder GetByUrlSlug(string customEntityDefinitionCode, string urlSlug);

        /// <summary>
        /// Fetch custom entities filtering on the UrlSlug property.
        /// </summary>
        /// <typeparam name="TDefinition">The definition type to fetch custom entities for.</typeparam>
        /// <param name="urlSlug">UrlSlug to filter the results on.</param>
        IContentRepositoryCustomEntityByUrlSlugQueryBuilder GetByUrlSlug<TDefinition>(string urlSlug) where TDefinition : ICustomEntityDefinition;

        /// <summary>
        /// Search for page entities, returning paged lists of data.
        /// </summary>
        IContentRepositoryCustomEntitySearchQueryBuilder Search();

        #endregion

        #region child entities

        /// <summary>
        /// Custom entity definitions are used to define the identity and
        /// behavior of a custom entity type. This includes meta data such
        /// as the name and description, but also the configuration of
        /// features such as whether the identity can contain a locale
        /// and whether versioning (i.e. auto-publish) is enabled.
        /// </summary>
        IContentRepositoryCustomEntityDefinitionsRepository Definitions();

        #endregion
    }
}
