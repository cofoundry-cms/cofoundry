using Cofoundry.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cofoundry.Web.Identity
{
    /// <summary>
    /// A helper class with shared functionality between controllers
    /// that manage the currently logged in users account.
    /// </summary>
    public class UserManagementControllerHelper
    {
        #region constructor

        private readonly IControllerResponseHelper _controllerResponseHelper;

        public UserManagementControllerHelper(
            IControllerResponseHelper controllerResponseHelper
            )
        {
            _controllerResponseHelper = controllerResponseHelper;
        }

        #endregion

        #region delete user

        public Task DeleteUserAsync(Controller controller, int userId)
        {
            var command = new DeleteUserCommand();
            command.UserId = userId;
            return _controllerResponseHelper.ExecuteIfValidAsync(controller, command);
        }

        #endregion
    }
}