using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Pages are a versioned entity and therefore have many page version
    /// records. At one time a page may only have one draft version, but
    /// can have many published versions; the latest published version is
    /// the one that is rendered when the page is published. 
    /// </summary>
    public partial class PageVersion : ICreateAuditable, IEntityVersion
    {
        public PageVersion()
        {
            PageVersionBlocks = new List<PageVersionBlock>();
            PagePublishStatusQueries = new List<PagePublishStatusQuery>();
        }

        /// <summary>
        /// Auto-incrementing primary key of the page version record 
        /// in the database.
        /// </summary>
        public int PageVersionId { get; set; }

        /// <summary>
        /// Id of the page this version is parented to.
        /// </summary>
        public int PageId { get; set; }

        /// <summary>
        /// Id of the template used to render this version.
        /// </summary>
        public int PageTemplateId { get; set; }

        /// <summary>
        /// A display-friendly version number that indicates
        /// it's position in the hisotry of all verions of a specific
        /// page. E.g. the first version for a page is version 1 and 
        /// the 2nd is version 2. The display version is unique per
        /// page.
        /// </summary>
        public int DisplayVersion { get; set; }

        /// <summary>
        /// The descriptive human-readable title of the page that is often 
        /// used in the html page title meta tag. Does not have to be
        /// unique.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The description of the content of the page. This is intended to
        /// be used in the description html meta tag.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Mapped from the domain enum WorkFlowStatus, this is the workflow 
        /// state of this version e.g. draft/published.
        /// </summary>
        public int WorkFlowStatusId { get; set; }

        /// <summary>
        /// Indicates whether the page should show in the auto-generated site map
        /// that gets presented to search engine robots.
        /// </summary>
        public bool ExcludeFromSitemap { get; set; }

        /// <summary>
        /// A title that can be used to share on social media via open 
        /// graph meta tags.
        /// </summary>
        public string OpenGraphTitle { get; set; }

        /// <summary>
        /// A description that can be used to share on social media via open 
        /// graph meta tags.
        /// </summary>
        public string OpenGraphDescription { get; set; }

        /// <summary>
        /// An image that can be used to share on social media via open 
        /// graph meta tags.
        /// </summary>
        public int? OpenGraphImageId { get; set; }

        /// <summary>
        /// An image that can be used to share on social media via open 
        /// graph meta tags.
        /// </summary>
        public ImageAsset OpenGraphImageAsset { get; set; }

        /// <summary>
        /// The template used to render this version.
        /// </summary>
        public PageTemplate PageTemplate { get; set; }

        /// <summary>
        /// Page content data to be rendered in the page template.
        /// </summary>
        public ICollection<PageVersionBlock> PageVersionBlocks { get; set; }

        /// <summary>
        /// The page this version is parented to.
        /// </summary>
        public Page Page { get; set; }

        /// <summary>
        /// Lookup cache used for quickly finding the correct version for a
        /// specific publish status query e.g. 'Latest', 'Published', 
        /// 'PreferPublished'.
        /// </summary>
        public ICollection<PagePublishStatusQuery> PagePublishStatusQueries { get; set; }

        #region ICreateAuditable

        /// <summary>
        /// The user that created the page version.
        /// </summary>
        public User Creator { get; set; }

        /// <summary>
        /// The date the page version was created.
        /// </summary>
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// The database id of the user that created the page version.
        /// </summary>
        public int CreatorId { get; set; }

        #endregion
    }
}
