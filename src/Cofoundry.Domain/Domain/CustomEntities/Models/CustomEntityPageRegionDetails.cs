using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityPageRegionDetails
    {
        public int PageTemplateRegionId { get; set; }

        public string Name { get; set; }

        public ICollection<CustomEntityVersionPageBlockDetails> Blocks { get; set; }
    }
}
