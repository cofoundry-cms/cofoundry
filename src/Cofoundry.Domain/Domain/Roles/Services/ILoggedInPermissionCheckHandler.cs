using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to mark up a query/command handler to make sure that a user is 
    /// logged into the system, use this as a simple alternative to having to specify
    /// more granular permissions in an application that don't require it.
    /// </summary>
    public interface ILoggedInPermissionCheckHandler : IPermissionCheckHandler
    {
    }
}
