using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// A helper class with shared functionality between controllers
    /// that manage the currently logged in users account.
    /// </summary>
    public class AccountManagementControllerHelper
    {
        private readonly IControllerResponseHelper _controllerResponseHelper;

        public AccountManagementControllerHelper(
            IControllerResponseHelper controllerResponseHelper
            )
        {
            _controllerResponseHelper = controllerResponseHelper;
        }

        /// <summary>
        /// Changes a users password, sending them an email notification if the operation 
        /// was successful.
        /// </summary>
        /// <param name="controller">Controller instance</param>
        /// <param name="vm">The IChangePasswordTemplate containing the data entered by the user.</param>
        /// <param name="userArea">
        /// The user area that the user belongs to. Usernames are only unique by user area 
        /// so all user commands need to be run against a specific user area.
        /// </param>
        /// <returns>The user id of the updated user if the action was successful; otheriwse null.</returns>
        public async Task ChangePasswordAsync(
            Controller controller,
            IChangePasswordViewModel vm,
            IUserAreaDefinition userArea
            )
        {
            if (controller == null) throw new ArgumentNullException(nameof(controller));
            if (userArea == null) throw new ArgumentNullException(nameof(userArea));
            if (vm == null) throw new ArgumentNullException(nameof(vm));

            if (controller.ModelState.IsValid)
            {
                var command = new UpdateUserPasswordByCredentialsCommand()
                {
                    UserAreaCode = userArea.UserAreaCode,
                    Username = vm.Username,
                    NewPassword = vm.NewPassword,
                    OldPassword = vm.OldPassword
                };

                await _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
            }
        }
    }
}