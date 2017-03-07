using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user with a specific username address in a specific user area 
    /// returning null if the user could not be found. Note that depending on the
    /// user area, the username may be a copy of the email address.
    /// </summary>
    public class GetUserMicroSummaryByUsernameQuery : IQuery<UserMicroSummary>
    {
        public GetUserMicroSummaryByUsernameQuery() { }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="username">This query must be run against a specific user area.</param>
        /// <param name="userAreaCode">The username to use to locate the user.</param>
        public GetUserMicroSummaryByUsernameQuery(string username, string userAreaCode)
        {
            Username = username;
            UserAreaCode = userAreaCode;
        }

        /// <summary>
        /// This query must be run against a specific user area.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The username to use to locate the user.
        /// </summary>
        public string Username { get; set; }
    }
}
