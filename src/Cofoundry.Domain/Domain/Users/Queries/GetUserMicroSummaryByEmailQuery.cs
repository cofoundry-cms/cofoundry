using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Finds a user with a specific email address in a specific user area 
    /// returning null if the user could not be found. Note that if the user
    /// area does not support email addresses then the email field will be empty.
    /// </summary>
    public class GetUserMicroSummaryByEmailQuery : IQuery<UserMicroSummary>
    {
        public GetUserMicroSummaryByEmailQuery() { }

        /// <summary>
        /// Initializes the query with parameters.
        /// </summary>
        /// <param name="email">The email address to use to locate the user.</param>
        /// <param name="userAreaCode">This query must be run against a specific user area.</param>
        public GetUserMicroSummaryByEmailQuery(string email, string userAreaCode)
        {
            Email = email;
            UserAreaCode = userAreaCode;
        }

        /// <summary>
        /// This query must be run against a specific user area.
        /// </summary>
        public string UserAreaCode { get; set; }

        /// <summary>
        /// The email address to use to locate the user.
        /// </summary>
        public string Email { get; set; }
    }
}
