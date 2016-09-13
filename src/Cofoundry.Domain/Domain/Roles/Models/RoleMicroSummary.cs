using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class RoleMicroSummary
    {
        public int RoleId { get; set; }

        public string Title { get; set; }

        public UserAreaMicroSummary UserArea { get; set; }
    }
}
