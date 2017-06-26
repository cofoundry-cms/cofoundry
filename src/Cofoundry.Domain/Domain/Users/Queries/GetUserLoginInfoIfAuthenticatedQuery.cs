using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;
using System.ComponentModel.DataAnnotations;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A query that gets information about a user if the specified credentials
    /// pass an authentication check
    /// </summary>
    public class GetUserLoginInfoIfAuthenticatedQuery : IQuery<UserLoginInfo>
    {
        /// <summary>
        /// The unique code of the user area match logins for. Note
        /// that usernames are unique per user area and therefore a given username
        /// may have an account for more than one user area.
        /// </summary>
        [Required]
        public string UserAreaCode { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }
    }
}
