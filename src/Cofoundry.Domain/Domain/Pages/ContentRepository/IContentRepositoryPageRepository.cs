using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// IContentRespository extension root for the Page entity.
    /// </summary>
    public interface IContentRepositoryPageRepository
    {
        #region queries

        /// <summary>
        /// Retrieve all pages in one query.
        /// </summary>
        IContentRepositoryPageGetAllQueryBuilder GetAll();

        /// <summary>
        /// Retrieve an page by a unique database id.
        /// </summary>
        /// <param name="pageId">PageId of the page to get.</param>
        IContentRepositoryPageByIdQueryBuilder GetById(int pageId);

        /// <summary>
        /// Retrieve a set of pages using a batch of database ids.
        /// The Cofoundry.Core dictionary extensions can be useful for 
        /// ordering the results e.g. results.FilterAndOrderByKeys(ids).
        /// </summary>
        /// <param name="pageIds">Range of PageIds of the pages to get.</param>
        IContentRepositoryPageByIdRangeQueryBuilder GetByIdRange(IEnumerable<int> pageIds);

        /// <summary>
        /// Retrieve a page for a specific path.
        /// </summary>
        IContentRepositoryPageByPathQueryBuilder GetByPath();

        /// <summary>
        /// Retrieve page data nested immediately inside a specific directory.
        /// </summary>
        /// <param name="directoryId">DirectoryId to query for pages with.</param>
        IContentRepositoryPageByDirectoryIdQueryBuilder GetByDirectoryId(int directoryId);

        /// <summary>
        /// Search for page entities, returning paged lists of data.
        /// </summary>
        IContentRepositoryPageSearchQueryBuilder Search();

        #endregion
    }
}
