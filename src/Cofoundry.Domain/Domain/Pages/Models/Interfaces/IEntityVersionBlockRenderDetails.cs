using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IEntityVersionPageBlockRenderDetails
    {
        PageBlockTypeTemplateSummary Template { get; set; }

        PageBlockTypeSummary BlockType { get; set; }

        IPageBlockTypeDisplayModel DisplayModel { get; set; }

        int EntityVersionPageBlockId { get; set; }
    }
}
