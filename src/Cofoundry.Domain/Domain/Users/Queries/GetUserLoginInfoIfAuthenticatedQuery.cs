using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public class GetUserLoginInfoIfAuthenticatedQuery : IQuery<UserLoginInfo>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
