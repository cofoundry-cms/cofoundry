using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class Page : ICreateAuditable
    {
        public Page()
        {
            PageGroupItems = new List<PageGroupItem>();
            PageTags = new List<PageTag>();
            PageVersions = new List<PageVersion>();
            PagePublishStatusQueries = new List<PagePublishStatusQuery>();
        }

        public int PageId { get; set; }

        public int PageDirectoryId { get; set; }

        public int? LocaleId { get; set; }

        public string UrlPath { get; set; }

        public int PageTypeId { get; set; }

        public bool IsDeleted { get; set; }

        public string CustomEntityDefinitionCode { get; set; }

        /// <summary>
        /// D = draft, P = Published
        /// </summary>
        public string PublishStatusCode { get; set; }

        /// <summary>
        /// The publish date should always be set if the status 
        /// is Published.
        /// </summary>
        public DateTime? PublishDate { get; set; }

        public virtual Locale Locale { get; set; }

        public virtual PageDirectory PageDirectory { get; set; }

        public virtual CustomEntityDefinition CustomEntityDefinition { get; set; }

        public virtual ICollection<PageGroupItem> PageGroupItems { get; set; }

        public virtual ICollection<PageTag> PageTags { get; set; }

        public virtual ICollection<PageVersion> PageVersions { get; set; }

        public virtual ICollection<PagePublishStatusQuery> PagePublishStatusQueries { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
