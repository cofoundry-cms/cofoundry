using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class UserDetails : UserSummary
    {
        public DateTime LastPasswordChangeDate { get; set; }

        public bool RequirePasswordChange { get; set; }
    }
}
