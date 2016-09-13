using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public interface IPermissionRestrictedQueryHandler : IPermissionCheckHandler
    {
    }

    public interface IPermissionRestrictedQueryHandler<TQuery, TResult> : IPermissionRestrictedQueryHandler
         where TQuery : IQuery<TResult>
    {
        IEnumerable<IPermissionApplication> GetPermissions(TQuery command);
    }
}
