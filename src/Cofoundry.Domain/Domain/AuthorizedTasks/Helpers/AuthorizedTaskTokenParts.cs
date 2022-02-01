using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Encapsulates the two pieces of data that are combined to make an authorization
    /// task token.
    /// </summary>
    public class AuthorizedTaskTokenParts
    {
        /// <summary>
        /// A unique identifier required to identify the authorized task.
        /// </summary>
        public Guid AuthorizedTaskId { get; set; }

        /// <summary>
        /// A cryptographically strong random code that is used to authenticate before 
        /// the action is permitted to be executed.
        /// </summary>
        public string AuthorizationCode { get; set; }
    }
}
