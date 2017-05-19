using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Cofoundry.Domain.Data;
using Cofoundry.Domain.CQS;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper used by ResetUserPasswordByUserIdCommandHandler and ResetUserPasswordByUsernameCommandHandler
    /// for shared functionality.
    /// </summary>
    public interface IResetUserPasswordCommandHelper
    {
        #region public methods

        Task ValidateCommandAsync(IResetUserPasswordCommand command, IExecutionContext executionContext);
        
        Task ResetPasswordAsync(User user, IResetUserPasswordCommand command, IExecutionContext executionContext);

        #endregion
    }
}
