using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntitySummary : IUpdateAudited, IPageRoute
    {
        public int CustomEntityId { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        public string Title { get; set; }

        public string UrlSlug { get; set; }

        /// <summary>
        /// The full path of the entity including directories and the locale. 
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Indicates if the entity has at least one published version and is currently
        /// viewable in the live site.
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// Indicates whether there is a draft version of this entity available.
        /// </summary>
        public bool HasDraft { get; set; }

        public int? Ordering { get; set; }

        /// <summary>
        /// Optional locale of the page.
        /// </summary>
        public ActiveLocale Locale { get; set; }

        public ICustomEntityDataModel Model { get; set; }

        public UpdateAuditData AuditData { get; set; }
    }
}
