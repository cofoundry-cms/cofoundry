using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A custom entity search that is not influenced by publish status. It returns 
    /// basic custom entity information with publish status and model data for the
    /// latest version. Designed to be used in the admin panel and not in a 
    /// version-sensitive context sach as a public webpage.
    /// </summary>
    public class SearchCustomEntitySummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<CustomEntitySummary>>
    {
        /// <summary>
        /// The six character code of the custom entity defintion to filter by. This
        /// is required; searching across multiple definitions is not supported by
        /// this query.
        /// </summary>
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// Text to filter on.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Locale id to filter the results by, if null then the is no filter (rather
        /// than entities with just a null locale.
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// By default a null localeId means 'any' locale, but for some
        /// functions we need to restrict the results to show only custom
        /// entities without a locale set.
        /// </summary>
        /// <remarks>
        /// This has been put in place for Custom Entity reordering where
        /// reordering has to be filtered to a locale. I imaging this might be
        /// implemented slightly differently once we start looking at how locales
        /// are implemented i.e. Should we always filter to a locale and never have an
        /// 'any' option.
        /// </remarks>
        public bool InterpretNullLocaleAsNone { get; set; }

        /// <summary>
        /// Filter to entities created on or after this date (inclusive).
        /// </summary>
        public DateTime? CreatedAfter { get; set; }

        /// <summary>
        /// Filter to entities created on or before this date (inclusive).
        /// </summary>
        public DateTime? CreatedBefore { get; set; }
    }
}
