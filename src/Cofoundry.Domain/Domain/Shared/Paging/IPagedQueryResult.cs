using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Represents the result of a paged query, including the original 
    /// query paging settings and stats about the results returned including
    /// the total page count and the total number of items. 
    /// </para>
    /// <para>
    /// This non-generic version of the interface is not intended to be implemented 
    /// directly.
    /// </para>
    /// </summary>
    public interface IPagedQueryResult
    {
        /// <summary>
        /// Total number of items in the result before paging was applied.
        /// </summary>
        int TotalItems { get; set; }

        /// <summary>
        /// Total number of pages.
        /// </summary>
        int PageCount { get; set; }

        /// <summary>
        /// Current (1-based) page number being returned.
        /// </summary>
        int PageNumber { get; set; }

        /// <summary>
        /// Number of items requested in the page (may not be equal to
        /// the actual number of items returned).
        /// </summary>
        int PageSize { get; set; }
    }

    /// <summary>
    /// <para>
    /// Represents the result of a paged query, including the original 
    /// query paging settings and stats about the results returned including
    /// the total page count and the total number of items. 
    /// </para>
    /// <para>
    /// If the result of the query needs to be mapped to another model type 
    /// you can use the ChangeType(newItems) method to convert the result.
    /// </para>
    /// </summary>
    public interface IPagedQueryResult<TResult> : IPagedQueryResult
    {
        /// <summary>
        /// The items returned.
        /// </summary>
        ICollection<TResult> Items { get; set; }
    }
}
