using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Extendable;
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
        /// Retrieve all pages in one query.
        /// </summary>
        IContentRepositoryPageGetAllQueryBuilder GetAll();

        /// <summary>
        /// Retieve a page by a unique database id.
        /// </summary>
        /// <param name="pageId">PageId of the page to get.</param>
        IAdvancedContentRepositoryPageByIdQueryBuilder GetById(int imageAssetId);

        /// <summary>
        /// Retieve a set of pages using a batch of database ids.
        /// The Cofoundry.Core dictionary extensions can be useful for 
        /// ordering the results e.g. results.FilterAndOrderByKeys(ids).
        /// </summary>
        /// <param name="pageIds">Range of PageIds of the pages to get.</param>
        IAdvancedContentRepositoryPageByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> imageAssetIds);

        /// <summary>
        /// Retieve a page for a specific path.
        /// </summary>
        IContentRepositoryPageByPathQueryBuilder GetByPath();

        /// <summary>
        /// Retieve data for "not found" (404) pages.
        /// </summary>
        IAdvancedContentRepositoryPageNotFoundQueryBuilder NotFound();

        /// <summary>
        /// Search for page entities, returning paged lists of data.
        /// </summary>
        IAdvancedContentRepositoryPageSearchQueryBuilder Search();

        /// <summary>
        /// Retrieve custom entity page data for a single custom 
        /// entity type.
        /// </summary>
        /// <param name="customEntityDefinitionCode">6 character CustomEntityDefinitionCode to query pages with.</param>
        IAdvancedContentRepositoryPageByCustomEntityDefinitionCodeQueryBuilder GetByCustomEntityDefinitionCode(string customEntityDefinitionCode);

        /// <summary>
        /// Retrieve custom entity page data for a custom entity id.
        /// </summary>
        /// <param name="customEntityId">CustomEntityId to query pages with.</param>
        IAdvancedContentRepositoryPageByCustomEntityIdQueryBuilder GetByCustomEntityId(int customEntityId);

        /// <summary>
        /// Retrieve custom entity page data for a set of custom 
        /// entity ids.
        /// </summary>
        /// <param name="customEntityIds">Range of CustomEntityIds to query pages with.</param>
        IAdvancedContentRepositoryPageByCustomEntityIdRangeQueryBuilder GetByCustomEntityIdRange(IEnumerable<int> customEntityIds);

        /// <summary>
        /// Retrieve page data nested immediately inside a specific directory.
        /// </summary>
        /// <param name="directoryId">DirectoryId to query for pages with.</param>
        IContentRepositoryPageByDirectoryIdQueryBuilder GetByDirectoryId(int customEntityId);

        /// <summary>
        /// Determines if a page path already exists. Page paths are made
        /// up of a locale, directory and url path; duplicates are not permitted.
        /// </summary>
        /// <param name="query">Query parameters.</param>
        Task<bool> IsPathUniqueAsync(IsPagePathUniqueQuery query);

        #endregion

        #region commands

        /// <summary>
        /// Adds a new custom entity with a draft version and optionally publishes it.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task AddAsync(AddCustomEntityCommand command);

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
        /// Command and queries for working with page versions.
        /// </summary>
        IAdvancedContentRepositoryPageVersionsRepository Versions();

        #endregion
    }
}
