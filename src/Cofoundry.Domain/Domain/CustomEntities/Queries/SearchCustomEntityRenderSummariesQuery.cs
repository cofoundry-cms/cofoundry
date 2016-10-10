using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    public class SearchCustomEntityRenderSummariesQuery
        : SimplePageableQuery
        , IQuery<PagedQueryResult<CustomEntityRenderSummary>>
    {
        [MaxLength(6)]
        [Required]
        public string CustomEntityDefinitionCode { get; set; }

        public SortDirection SortDirection { get; set; }

        public CustomEntityQuerySortType SortBy { get; set; }

        /// <summary>
        /// Locale id to filter the reults by, if null then only entities
        /// with a null locale are shown
        /// </summary>
        public int? LocaleId { get; set; }
    }
}
