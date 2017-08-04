using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public partial class PageVersionBlock : ICreateAuditable, IEntityOrderable, IEntityVersionPageBlock
    {
        public int PageVersionBlockId { get; set; }
        public int PageVersionId { get; set; }
        public int PageTemplateRegionId { get; set; }
        public int PageBlockTypeId { get; set; }
        public string SerializedData { get; set; }
        public int Ordering { get; set; }
        public DateTime UpdateDate { get; set; }
        public int? PageBlockTypeTemplateId { get; set; }

        public virtual PageTemplateRegion PageTemplateRegion { get; set; }
        public virtual PageBlockType PageBlockType { get; set; }
        public virtual PageVersion PageVersion { get; set; }
        public virtual PageBlockTypeTemplate PageBlockTypeTemplate { get; set; }

        #region ICreateAuditable

        public System.DateTime CreateDate { get; set; }
        public int CreatorId { get; set; }
        public virtual User Creator { get; set; }

        #endregion

        public int GetVersionBlockId()
        {
            return PageVersionBlockId;
        }
    }
}
