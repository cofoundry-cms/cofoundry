using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageVersion : ICreateAuditable, IEntityVersion
    {
        public PageVersion()
        {
            PageVersionBlocks = new List<PageVersionBlock>();
            ChildPageVersions = new List<PageVersion>();
        }

        public int PageVersionId { get; set; }
        public int PageId { get; set; }
        public int PageTemplateId { get; set; }
        public int? BasedOnPageVersionId { get; set; }
        public string Title { get; set; }
        public string MetaDescription { get; set; }
        public int WorkFlowStatusId { get; set; }
        public bool IsDeleted { get; set; }
        public bool ExcludeFromSitemap { get; set; }
        public string OpenGraphTitle { get; set; }
        public string OpenGraphDescription { get; set; }
        public int? OpenGraphImageId { get; set; }
        public virtual ImageAsset OpenGraphImageAsset { get; set; }
        public virtual PageTemplate PageTemplate { get; set; }
        public virtual ICollection<PageVersionBlock> PageVersionBlocks { get; set; }
        public virtual Page Page { get; set; }
        public virtual ICollection<PageVersion> ChildPageVersions { get; set; }
        public virtual PageVersion BasedOnPageVersion { get; set; }
        
        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
