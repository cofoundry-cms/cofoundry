using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Search page data returning the PageSummary projection, which is primarily used
    /// to display lists of page information in the admin panel. The query isn't version 
    /// specific and should not be used to render content out to a live page because some of
    /// the pages returned may be unpublished.
    /// </summary>
    public class SearchPageSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<PageSummary>>
    {
        /// <summary>
        /// Basic text filtering which only filters on url slug and title. Full-text filtering will
        /// be added in a later release.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Filters by tags, which can be provided as a space or comma separated
        /// list. Tags with multiple words or commas can be escaped with single or
        /// double quotation marks.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Locale id to filter the results by, if null then only entities
        /// with a null locale are shown
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// By default the query will filter to published pages only. Use this option
        /// to change this behaviour.
        /// </summary>
        public PublishStatus? PublishStatus { get; set; }

        /// <summary>
        /// Optionally filter by the directory the page is parented to.
        /// </summary>
        public int? PageDirectoryId { get; set; }

        /// <summary>
        /// Optionally filter by the template the latest version of this page uses.
        /// </summary>
        public int? PageTemplateId { get; set; }

        [Obsolete("Note that the page grouping system is not fully implemented will be revised in an upcomming release.")]
        public int? PageGroupId { get; set; }

        /// <summary>
        /// Option to reverse the direction of the sort.
        /// </summary>
        public SortDirection SortDirection { get; set; }

        /// <summary>
        /// The default sort is title ordering, but it can be changed
        /// with this option.
        /// </summary>
        public PageQuerySortType SortBy { get; set; }
    }
}
