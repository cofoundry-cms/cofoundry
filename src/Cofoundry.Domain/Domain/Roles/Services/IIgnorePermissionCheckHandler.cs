using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// This can be used to mark up a query/command handler to tell the system
    /// that it doesn't require permission checking. Without this an exception would be thrown 
    /// to prevent a developer from forgetting to add permissions handling to their domain layer.
    /// </summary>
    public interface IIgnorePermissionCheckHandler : IPermissionCheckHandler
    {
    }
}
