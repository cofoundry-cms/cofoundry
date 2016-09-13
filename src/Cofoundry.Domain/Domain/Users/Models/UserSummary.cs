using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class UserSummary : UserMicroSummary, ICreateAudited
    {
        public string Username { get; set; }

        public RoleMicroSummary Role { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public CreateAuditData AuditData { get; set; }
    }
}
