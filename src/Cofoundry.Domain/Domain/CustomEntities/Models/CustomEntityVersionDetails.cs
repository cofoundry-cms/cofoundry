using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class CustomEntityVersionDetails : CustomEntityVersionSummary
    {
        public ICustomEntityDataModel Model { get; set; }

        public IEnumerable<CustomEntityPage> Pages { get; set; }
    }
}
