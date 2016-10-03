using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Used to ensure a user has a specific set of permissions before the handler is executed
    /// </summary>
    public interface IPermissionRestrictedQueryHandler : IPermissionCheckHandler
    {
    }

    /// <summary>
    /// Used to ensure a user has a specific set of permissions before the handler is executed
    /// </summary>
    public interface IPermissionRestrictedQueryHandler<TQuery, TResult> : IPermissionRestrictedQueryHandler
         where TQuery : IQuery<TResult>
    {
        IEnumerable<IPermissionApplication> GetPermissions(TQuery query);
    }
}
