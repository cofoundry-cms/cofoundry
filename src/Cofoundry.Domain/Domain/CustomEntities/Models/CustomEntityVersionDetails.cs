using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionDetails : CustomEntityVersionSummary
    {
        public ICustomEntityVersionDataModel Model { get; set; }

        public IEnumerable<CustomEntityDetailsPage> Pages { get; set; }
    }
}
