using System;
using System.Collections.Generic;
using System.Linq;
using Cofoundry.Domain.CQS;
using Cofoundry.Domain;
using Cofoundry.Core.Mail;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// A helper class with shared functionality between controllers
    /// that manage the currently logged in users account.
    /// </summary>
    public class AccountManagementControllerHelper
    {
        #region constructor

        private readonly IQueryExecutor _queryExecutor;
        private readonly ICommandExecutor _commandExecutor;
        private readonly IUserContextService _userContextService;
        private readonly IMailService _mailService;
        private readonly IControllerResponseHelper _controllerResponseHelper;

        public AccountManagementControllerHelper(
            IQueryExecutor queryExecutor,
            IMailService mailService,
            ICommandExecutor commandExecutor,
            IUserContextService userContextService,
            IControllerResponseHelper controllerResponseHelper
            )
        {
            _queryExecutor = queryExecutor;
            _commandExecutor = commandExecutor;
            _userContextService = userContextService;
            _mailService = mailService;
            _controllerResponseHelper = controllerResponseHelper;
        }

        #endregion
        
        #region change password

        public async Task InitViewModelAsync(IChangePasswordViewModel vm)
        {
            var cx = await _userContextService.GetCurrentContextAsync();
            vm.IsPasswordChangeRequired = cx.IsPasswordChangeRequired;
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
                var command = new UpdateUnauthenticatedUserPasswordCommand()
                {
                    UserAreaCode = userArea.UserAreaCode,
                    Username = vm.Username,
                    NewPassword = vm.NewPassword,
                    OldPassword = vm.OldPassword
                };

                await _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
            }
        }

        #endregion
    }
}