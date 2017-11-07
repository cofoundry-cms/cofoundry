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
            PagePublishStatusQueries = new List<PagePublishStatusQuery>();
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

        public ImageAsset OpenGraphImageAsset { get; set; }
        public PageTemplate PageTemplate { get; set; }
        public ICollection<PageVersionBlock> PageVersionBlocks { get; set; }
        public Page Page { get; set; }
        public ICollection<PageVersion> ChildPageVersions { get; set; }
        public PageVersion BasedOnPageVersion { get; set; }
        public ICollection<PagePublishStatusQuery> PagePublishStatusQueries { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion
    }
}
