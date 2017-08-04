using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityDetails : ICreateAudited
    {
        public int CustomEntityId { get; set; }

        public ActiveLocale Locale { get; set; }

        public string UrlSlug { get; set; }

        /// <summary>
        /// Indicates if ther page has at least one published version and is currently
        /// viewable in the live site.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Indicates whether there is a draft version of this entity available.
        /// </summary>
        public bool HasDraft { get; set; }

        /// <summary>
        /// The full path of the default details page. 
        /// </summary>
        public string FullPath { get; set; }

        public CustomEntityVersionDetails LatestVersion { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
