using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class SearchCustomEntitySummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<CustomEntitySummary>>
    {
        [Required]
        [MaxLength(6)]
        public string CustomEntityDefinitionCode { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Locale id to filter the rssults by, if null then the is no filter (rather
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
    }
}
