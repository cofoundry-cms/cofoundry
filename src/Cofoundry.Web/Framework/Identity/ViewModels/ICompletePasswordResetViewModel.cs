using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Identity
{
    public interface ICompletePasswordResetViewModel
    {
        string NewPassword { get; set; }

        string ConfirmNewPassword { get; set; }

        string UserPasswordResetRequestId { get; set; }

        string Token { get; set; }
    }
}