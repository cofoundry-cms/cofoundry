using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A page summary contains information used when listing pages in the
    /// admin panel. The model isn't version specific and should not be used 
    /// to render content out to a live page, since the page may not be 
    /// published.
    /// </summary>
    public class PageSummary : ICreateAudited, IPageRoute
    {
        #region properties shared with page route

        public int PageId { get; set; }
        
        /// <summary>
        /// The path of the page within the directory.
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// The full path of the page including directories and the locale. 
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Indicates if the page has at least one published version and is currently
        /// viewable in the live site.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Indicates whether there is a draft version of this page available.
        /// </summary>
        public bool HasDraft { get; set; }

        /// <summary>
        /// The title of the page for the currently published version, falling
        /// back to the draft version is there is no published version.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Optional locale of the page.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        /// <summary>
        /// The page could be a generic page or could have some sort of
        /// special function e.g. NotFound, CustomEntityDetails
        /// </summary>
        public PageType PageType { get; set; }

        public string CustomEntityName { get; set; }

        #endregion

        #region properties unique to this class

        public string[] Tags { get; set; }

        public CreateAuditData AuditData { get; set; }

        #endregion
    }
}
