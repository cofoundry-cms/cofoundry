using System.Security.Claims;

namespace Cofoundry.Web.Internal
{
    /// <summary>
    /// Claim type constants used by Cofoundry auth.
    /// </summary>
    public class CofoundryClaimTypes
    {
        /// <summary>
        /// The claim type Cofoundry uses to reference the UserId
        /// claim.
        /// </summary>
        public const string UserId = ClaimTypes.NameIdentifier;

        /// <summary>
        /// The claim type Cofoundry uses to reference the SecurityStamp
        /// claim.
        /// </summary>
        public const string SecurityStamp = "Cofoundry.SecurityStamp";

        /// <summary>
        /// The claim type Cofoundry uses to reference the UserAreaCode
        /// claim.
        /// </summary>
        public const string UserAreaCode = "Cofoundry.UserAreaCode";
    }
}