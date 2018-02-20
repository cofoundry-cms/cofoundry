using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Identity
{
    public interface IChangePasswordViewModel
    {
        string Username { get; set; }

        string OldPassword { get; set; }

        string NewPassword { get; set; }

        string ConfirmNewPassword { get; set; }

        /// <summary>
        /// Indicates if the password change is madatory (e.g. after first login)
        /// </summary>
        bool IsPasswordChangeRequired { get; set; }
    }
}