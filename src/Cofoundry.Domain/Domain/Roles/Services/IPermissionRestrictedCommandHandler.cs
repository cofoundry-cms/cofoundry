using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    public interface IPermissionRestrictedCommandHandler : IPermissionCheckHandler
    {
    }

    public interface IPermissionRestrictedCommandHandler<TCommand> : IPermissionRestrictedCommandHandler 
        where TCommand : ICommand
    {
        IEnumerable<IPermissionApplication> GetPermissions(TCommand command);
    }
}
