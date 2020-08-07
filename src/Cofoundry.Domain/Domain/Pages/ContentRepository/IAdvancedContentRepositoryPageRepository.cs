using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IAdvancedContentRespository extension root for the Page entity.
    /// </summary>
    public interface IAdvancedContentRepositoryPageRepository
    {
        #region queries

        /// <summary>
        /// Retrieve all pages in one query.
        /// </summary>
        IContentRepositoryPageGetAllQueryBuilder GetAll();

        /// <summary>
        /// Retrieve a page by a unique database id.
        /// </summary>
        /// <param name="pageId">PageId of the page to get.</param>
        IAdvancedContentRepositoryPageByIdQueryBuilder GetById(int pageId);

        /// <summary>
        /// Retrieve a set of pages using a batch of database ids.
        /// The Cofoundry.Core dictionary extensions can be useful for 
        /// ordering the results e.g. results.FilterAndOrderByKeys(ids).
        /// </summary>
        /// <param name="pageIds">Range of PageIds of the pages to get.</param>
        IAdvancedContentRepositoryPageByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> imageAssetIds);

        /// <summary>
        /// Retrieve a page for a specific path.
        /// </summary>
        IContentRepositoryPageByPathQueryBuilder GetByPath();

        /// <summary>
        /// Retrieve data for "not found" (404) pages.
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
        IDomainRepositoryQueryContext<bool> IsPathUnique(IsPagePathUniqueQuery query);

        #endregion

        #region commands

        /// <summary>
        /// Adds a new page with a draft version and optionally publishes it.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        /// <returns>Id of the newly created page.</returns>
        Task<int> AddAsync(AddPageCommand command);

        /// <summary>
        /// Creates a new page, copying from an existing page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task DuplicateAsync(DuplicatePageCommand command);

        /// <summary>
        /// Publishes a page. If the page is already published and
        /// a date is specified then the publish date will be updated.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task PublishAsync(PublishPageCommand command);

        /// <summary>
        /// Sets the status of a page to un-published, but does not
        /// remove the publish date, which is preserved so that it
        /// can be used as a default when the user chooses to publish
        /// again.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UnPublishAsync(UnPublishPageCommand command);

        /// <summary>
        /// Updates page properties that aren't specific to a
        /// version.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateAsync(UpdatePageCommand command);

        /// <summary>
        /// Updates the url of a page.
        /// </summary>
        /// <param name="command">Command parameters.</param>
        Task UpdateUrlAsync(UpdatePageUrlCommand command);

        /// <summary>
        /// Deletes a page and all associated versions permanently.
        /// </summary>
        /// <param name="pageId">Id of the page to delete.</param>
        Task DeleteAsync(int pageId);

        #endregion

        #region child entities

        /// <summary>
        /// Pages are a versioned entity and therefore have many page version
        /// records. At one time a page may only have one draft version, but
        /// can have many published versions; the latest published version is
        /// the one that is rendered when the page is published. 
        /// </summary>
        IAdvancedContentRepositoryPageVersionsRepository Versions();

        #endregion
    }
}
