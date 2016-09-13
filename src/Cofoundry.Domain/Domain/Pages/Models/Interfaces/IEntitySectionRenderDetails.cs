using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public interface IEntitySectionRenderDetails<TModuleRenderDetails> where TModuleRenderDetails : IEntityVersionPageModuleRenderDetails
    {
        int PageTemplateSectionId { get; set; }

        string Name { get; set; }

        TModuleRenderDetails[] Modules { get; set; }
    }
}
