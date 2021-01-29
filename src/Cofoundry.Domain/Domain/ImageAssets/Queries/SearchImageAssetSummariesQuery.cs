using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Searches image assets based on simple filter criteria and 
    /// returns a paged set of summary results. 
    /// </summary>
    public class SearchImageAssetSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<ImageAssetSummary>>
    {
        /// <summary>
        /// Restrict result to images labelled with these tags.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Restrict result to images with exactly this width.
        /// </summary>
        public int? Width { get; set; }

        /// <summary>
        /// Restrict result to images with exactly this height.
        /// </summary>
        public int? Height { get; set; }

        /// <summary>
        /// Restrict result to images of at least this width (inclusive).
        /// </summary>
        public int? MinWidth { get; set; }

        /// <summary>
        /// Restrict result to images of at least this height (inclusive).
        /// </summary>
        public int? MinHeight { get; set; }
    }
}
