using Cofoundry.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public void DeleteUser(Controller controller, int userId)
        {
            var command = new DeleteUserCommand();
            command.UserId = userId;
            _controllerResponseHelper.ExecuteIfValid(controller, command);
        }

        #endregion
    }
}