using System;

namespace Cofoundry.Domain
{
    public class UserAccountRecoveryTokenParts
    {
        /// <summary>
        /// A unique identifier required to identify the account
        /// recovery request.
        /// </summary>
        public Guid UserAccountRecoveryRequestId { get; set; }

        /// <summary>
        /// A crypographically strong random string used to authenticate when 
        /// completing the account recovery.
        /// </summary>
        public string AuthorizationCode { get; set; }
    }
}
