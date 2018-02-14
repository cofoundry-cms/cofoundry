using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A lighter weight version of PageRegionRenderDetails
    /// without full display model mapping for page blocks. Includes 
    /// only raw data model data.
    /// </summary>
    public class PageRegionDetails
    {
        public int PageTemplateRegionId { get; set; }

        public string Name { get; set; }

        public ICollection<PageVersionBlockDetails> Blocks { get; set; }
    }
}
