using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Pages represent the dynamically navigable pages of your website. Each page uses a template 
    /// which defines the regions of content that users can edit. Pages are a versioned entity and 
    /// therefore have many page version records. At one time a page may only have one draft 
    /// version, but can have many published versions; the latest published version is the one that 
    /// is rendered when the page is published. 
    /// </summary>
    public partial class Page : ICreateAuditable
    {
        public Page()
        {
            PageGroupItems = new List<PageGroupItem>();
            PageTags = new List<PageTag>();
            PageVersions = new List<PageVersion>();
            PagePublishStatusQueries = new List<PagePublishStatusQuery>();
        }

        /// <summary>
        /// The auto-incrementing database id of the page.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// The id of the directory this page is in.
        /// </summary>
        public int PageDirectoryId { get; set; }

        /// <summary>
        /// Optional id of the locale if used in a localized site.
        /// </summary>
        public int? LocaleId { get; set; }

        /// <summary>
        /// The path of the page within the directory. This must be
        /// unique within the directory the page is parented to.
        /// </summary>
        public string UrlPath { get; set; }

        /// <summary>
        /// Most pages are generic pages but they could have some sort of
        /// special function e.g. NotFound, CustomEntityDetails. This is the
        /// numeric representation of the domain PageType enum.
        /// </summary>
        public int PageTypeId { get; set; }

        /// <summary>
        /// If this is of PageType.CustomEntityDetails, this is used
        /// to look up the routing.
        /// </summary>
        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// D = draft, P = Published. Use the PublishStatusMapper in the
        /// domain to map this.
        /// </summary>
        public string PublishStatusCode { get; set; }

        /// <summary>
        /// The publish date should always be set if the status 
        /// is Published.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        /// <summary>
        /// Optional locale of the page if used in a localized site.
        /// </summary>
        public virtual Locale Locale { get; set; }

        /// <summary>
        /// The directory this page is in.
        /// </summary>
        public virtual PageDirectory PageDirectory { get; set; }

        /// <summary>
        /// If this is of PageType.CustomEntityDetails, this is used
        /// to look up the routing.
        /// </summary>
        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        public virtual ICollection<PageGroupItem> PageGroupItems { get; set; }

        /// <summary>
        /// Tags can be used to categorize an entity.
        /// </summary>
        public virtual ICollection<PageTag> PageTags { get; set; }

        /// <summary>
        /// Pages are a versioned entity and therefore have many page version
        /// records. At one time a page may only have one draft version, but
        /// can have many published versions; the latest published version is
        /// the one that is rendered when the page is published. 
        /// </summary>
        public virtual ICollection<PageVersion> PageVersions { get; set; }

        /// <summary>
        /// Lookup cache used for quickly finding the correct version for a
        /// specific publish status query e.g. 'Latest', 'Published', 'PreferPublished'
        /// </summary>
        public virtual ICollection<PagePublishStatusQuery> PagePublishStatusQueries { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
