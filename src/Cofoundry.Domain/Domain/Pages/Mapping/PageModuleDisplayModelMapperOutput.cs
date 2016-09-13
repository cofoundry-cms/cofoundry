using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageModuleDisplayModelMapperOutput
    {
        public int VersionModuleId { get; set; }
        public IPageModuleDisplayModel DisplayModel { get; set; }
    }
}
