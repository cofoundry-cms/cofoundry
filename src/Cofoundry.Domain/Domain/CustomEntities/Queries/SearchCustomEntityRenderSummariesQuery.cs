using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// <para>
    /// Search for custom entities of a specific type and return them
    /// as a render summary projection which is a general-purpose projection 
    /// of a custom entity with version specific data, including a deserialized 
    /// data model.
    /// </para>
    /// <para>
    /// The query is version-sensitive and defaults to returning published 
    /// versions only, but this behavior can be controlled by the PublishStatus 
    /// query property.
    /// </para>
    /// </summary>
    public class SearchCustomEntityRenderSummariesQuery
        : SimplePageableQuery
        , IQuery<PagedQueryResult<CustomEntityRenderSummary>>
    {
        /// <summary>
        /// The six character code of the custom entity defintion to filter by. This
        /// is required; searching across multiple definitions is not supported by
        /// this query.
        /// </summary>
        [MaxLength(6)]
        [Required]
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Optional direction on which to sort the results. If null
        /// then then default sort ordering is applied.
        /// </summary>
        public SortDirection? SortDirection { get; set; }

        /// <summary>
        /// Indicates which property or behavior should
        /// be used to determine the sort order.
        /// </summary>
        public CustomEntityQuerySortType SortBy { get; set; }

        /// <summary>
        /// Locale id to filter the results by, if null then only entities
        /// with a null locale are shown
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// Used to determine which version of the custom entities to include 
        /// data for. This defaults to Published, meaning that only published 
        /// custom entities will be returned.
        /// </summary>
        public PublishStatusQuery PublishStatus { get; set; }
    }
}
