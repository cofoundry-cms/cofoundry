using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IEntityRegionRenderDetails<TBlockRenderDetails> where TBlockRenderDetails : IEntityVersionPageBlockRenderDetails
    {
        int PageTemplateRegionId { get; set; }

        string Name { get; set; }

        ICollection<TBlockRenderDetails> Blocks { get; set; }
    }
}
