using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Determines if a page path already exists. Page paths are made
    /// up of a locale, directory and url slug; duplicates are not permitted.
    /// </summary>
    public class IsPagePathUniqueQuery : IQuery<bool>
    {
        /// <summary>
        /// Database id of a page to exclude from the check. Used when 
        /// checking an existing page for uniqueness.
        /// </summary>
        public int? PageId { get; set; }

        /// <summary>
        /// The path of the page within the directory. 
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// The directory the page is parented to.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Optional id of the locale if used in a localized site.
        /// </summary>
        public int? LocaleId { get; set; }
    }
}
