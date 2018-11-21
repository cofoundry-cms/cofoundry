using Cofoundry.Domain.CQS;
using Cofoundry.Domain.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Send a notification to the user to let them know their 
        /// password has been changed. The template is built using the
        /// registered UserMailTemplateBuilderFactory for the users
        /// user area.
        /// </summary>
        /// <param name="user">The user to send the notification to.</param>
        Task SendPasswordChangedNotification(User user);
    }
}
