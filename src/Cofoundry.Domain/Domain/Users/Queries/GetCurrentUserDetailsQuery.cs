using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Gets a UserSummary object representing the currently logged in 
    /// user. If the user is not logged in then null is returned.
    /// </summary>
    public class GetCurrentUserDetailsQuery : IQuery<UserDetails>
    {
    }
}
