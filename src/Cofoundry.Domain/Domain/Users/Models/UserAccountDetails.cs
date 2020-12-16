using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <remarks>
    /// Could this be better represented by other projections? Seems
    /// to be only used in the account editing screen - can't we use
    /// UserDetails for that?
    /// </remarks>
    public class UserAccountDetails : UserMicroSummary, ICreateAudited
    {
        public DateTime LastPasswordChangeDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public DateTime? PreviousLoginDate { get; set; }

        public CreateAuditData AuditData { get; set; }

        public bool RequirePasswordChange { get; set; }
    }
}
