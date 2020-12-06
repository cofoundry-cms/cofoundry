using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the custom entities.
    /// </summary>
    public interface IAdvancedContentRepositoryCustomEntityRepository
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
        IAdvancedContentRepositoryCustomEntityByIdQueryBuilder GetById(int customEntityId);

        /// <summary>
        /// Retrieve a set of pages using a batch of database ids.
        /// The Cofoundry.Core dictionary extensions can be useful for 
        /// ordering the results e.g. results.FilterAndOrderByKeys(ids).
        /// </summary>
        /// <param name="pageIds">Range of PageIds of the pages to get.</param>
        IAdvancedContentRepositoryCustomEntityByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> pageIds);

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
        IAdvancedContentRepositoryCustomEntitySearchQueryBuilder Search();

        /// <summary>
        /// Query to determine if the UrlSlug property for a custom entity is invalid because it
        /// already exists. If the custom entity defition has ForceUrlSlugUniqueness 
        /// set to true then duplicates are not permitted, otherwise true will always
        /// be returned.
        /// </summary>
        IDomainRepositoryQueryContext<bool> IsUrlSlugUnique(IsCustomEntityUrlSlugUniqueQuery query);

        /// <summary>
        /// Gets custom entity page data by a url path/route. Note that this
        /// is specific to custom entity pages.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityByPathQueryBuilder GetByPath();

        #endregion

        #region commands

        /// <summary>
        /// Adds a new custom entity with a draft version and optionally publishes it.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created custom entity.</returns>
        Task<int> AddAsync(AddCustomEntityCommand command);

        /// <summary>
        /// Creates a new custom entity, copying from an existing custom entity.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task DuplicateAsync(DuplicateCustomEntityCommand command);

        /// <summary>
        /// Publishes a custom entity. If the custom entity is already published and
        /// a date is specified then the publish date will be updated.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task PublishAsync(PublishCustomEntityCommand command);

        /// <summary>
        /// Sets the status of a custom entity to un-published, but does not
        /// remove the publish date, which is preserved so that it
        /// can be used as a default when the user chooses to publish
        /// again.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UnPublishAsync(UnPublishCustomEntityCommand command);

        /// <summary>
        /// Updates the UrlSlug and locale of a custom entity which often forms
        /// the identity of the entity and can form part fo the url when used in
        /// custom entity pages. This is a specific action that can
        /// have specific side effects such as breaking page links outside
        /// of the CMS.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateUrlAsync(UpdateCustomEntityUrlCommand command);

        /// <summary>
        /// Reorders a set of custom entities. The custom entity definition must implement 
        /// IOrderableCustomEntityDefintion to be able to set ordering.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task ReOrderAsync(ReOrderCustomEntitiesCommand command);

        /// <summary>
        /// Changes the order of a single custom entity. The custom entity 
        /// definition must implement IOrderableCustomEntityDefintion to be 
        /// able to set ordering.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateOrderingPositionAsync(UpdateCustomEntityOrderingPositionCommand command);

        /// <summary>
        /// Deletes a custom entity and all associated versions permanently.
        /// </summary>
        /// <param name="customEntityId">Database id of the custom entity to delete.</param>
        Task DeleteAsync(int customEntityId);

        #endregion

        #region child entities

        /// <summary>
        /// <para>
        /// Custom entities can have one or more version, with a collection
        /// of versions representing the change history of custom entity
        /// data. 
        /// </para>
        /// <para>
        /// Only one draft version can exist at any one time, and 
        /// only one version may be published at any one time. Although
        /// you can revert to a previous version, this is done by copying
        /// the old version data to a new version, so that a full history is
        /// always maintained.
        /// </para>
        /// </summary>
        IAdvancedContentRepositoryCustomEntityVersionsRepository Versions();

        /// <summary>
        /// Custom entity definitions are used to define the identity and
        /// behavior of a custom entity type. This includes meta data such
        /// as the name and description, but also the configuration of
        /// features such as whether the identity can contain a locale
        /// and whether versioning (i.e. auto-publish) is enabled.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityDefinitionsRepository Definitions();

        /// <summary>
        /// A data model schema contains meta information about a custom entity data 
        /// model, including UI display details and validation attributes for each public 
        /// property. This is typically used for expressing these entities in dynamically 
        /// generated parts of the UI e.g. edit forms and lists.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityDataModelSchemasRepository DataModelSchemas();

        /// <summary>
        /// Custom entity routing rules respresent the dynamic routing 
        /// rules used to work out which custom  entity should be displayed 
        /// in a custom entity details page. E.g. a rule with a format of
        /// "{Id}/{UrlSlug}" uses those parameters to identify a custom entity.
        /// </summary>
        IAdvancedContentRepositoryCustomEntityRoutingRulesRepository RoutingRules();

        #endregion
    }
}
