using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Helper used by in password update commands for shared functionality.
    /// </summary>
    public interface IPasswordUpdateCommandHelper
    {
        void ValidateUserArea(IUserAreaDefinition userArea);

        void ValidatePermissions(IUserAreaDefinition userArea, IExecutionContext executionContext);

        void UpdatePassword(string newPassword, User user, IExecutionContext executionContext);
    }
}
