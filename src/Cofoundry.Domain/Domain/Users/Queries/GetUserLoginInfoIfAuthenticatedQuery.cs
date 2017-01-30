using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// A query that gets information about a user if the specified credentials
    /// pass an authentication check
    /// </summary>
    public class GetUserLoginInfoIfAuthenticatedQuery : IQuery<UserLoginInfo>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
