using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class PageBlockTypeSummary
    {
        public int PageBlockTypeId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FileName { get; set; }

        public ICollection<PageBlockTypeTemplateSummary> Templates { get; set; }
    }
}
