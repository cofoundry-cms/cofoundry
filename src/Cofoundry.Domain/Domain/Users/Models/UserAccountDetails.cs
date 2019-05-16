using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    public class UserAccountDetails : UserMicroSummary, ICreateAudited
    {
        /// <summary>
        /// The date the password was last changed or the that the password
        /// was first set (account create date)
        /// </summary>
        public DateTime LastPasswordChangeDate { get; set; }

        /// <summary>
        /// The date the user last logged in, if ever.
        /// </summary>
        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// The date the user logged in before the LastLoginDate.
        /// </summary>
        /// <remarks>
        /// We can consider dropping this now we have a LoginLog table, I don't think it's
        /// used for anything else
        /// </remarks>
        public DateTime? PreviousLoginDate { get; set; }

        /// <summary>
        /// True if a password change is required, this is set to true when an account is
        /// first created.
        /// </summary>
        public bool RequirePasswordChange { get; set; }

        /// <summary>
        /// A flag to indicate if the users email address has been confirmed via a 
        /// sign-up notification.
        /// </summary>
        public bool IsEmailConfirmed { get; set; }

        /// <summary>
        /// Data detailing who created the user and when.
        /// </summary>
        public CreateAuditData AuditData { get; set; }
    }
}
