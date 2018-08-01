using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain.Data
{
    /// <summary>
    /// Abstraction to allow CustomEntityPageBlock and PageVersionBlock to
    /// be treated as the same thing.
    /// </summary>
    public interface IEntityVersionPageBlock
    {
        int PageTemplateRegionId { get; set; }

        int PageBlockTypeId { get; set; }

        int? PageBlockTypeTemplateId { get; set; }

        string SerializedData { get; set; }

        int Ordering { get; set; }

        PageTemplateRegion PageTemplateRegion { get; set; }

        PageBlockType PageBlockType { get; set; }

        PageBlockTypeTemplate PageBlockTypeTemplate { get; set; }

        int GetVersionBlockId();
    }
}
