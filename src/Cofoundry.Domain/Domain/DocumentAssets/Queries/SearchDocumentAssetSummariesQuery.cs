using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Searches document assets based on simple filter criteria and 
    /// returns a paged set of summary results. 
    /// </summary>
    public class SearchDocumentAssetSummariesQuery : SimplePageableQuery, IQuery<PagedQueryResult<DocumentAssetSummary>>
    {
        /// <summary>
        /// Restrict result to documents labelled with these tags.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// Restrict result to documents only with this file extension.
        /// The leading dot is optional.
        /// </summary>
        public string FileExtension { get; set; }

        /// <summary>
        /// Restrict result to documents only with these file extensions.
        /// This should be a comma or space separated list of file
        /// extensions; the leading dot is optional.
        /// </summary>
        public string FileExtensions { get; set; }
    }
}
