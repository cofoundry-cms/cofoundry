using System;
using System.Collections.Generic;

namespace Cofoundry.Domain.Data
{
    public class CustomEntityVersionPageBlock : IEntityOrderable, IEntityVersionPageBlock
    {
        public int CustomEntityVersionPageBlockId { get; set; }

        public int CustomEntityVersionId { get; set; }

        public int PageTemplateRegionId { get; set; }

        public int PageBlockTypeId { get; set; }

        public int? PageBlockTypeTemplateId { get; set; }

        public string SerializedData { get; set; }

        public int Ordering { get; set; }

        public virtual CustomEntityVersion CustomEntityVersion { get; set; }

        public virtual PageTemplateRegion PageTemplateRegion { get; set; }

        public virtual PageBlockType PageBlockType { get; set; }

        public virtual PageBlockTypeTemplate PageBlockTypeTemplate { get; set; }

        public int GetVersionBlockId()
        {
            return CustomEntityVersionPageBlockId;
        }
    }
}
