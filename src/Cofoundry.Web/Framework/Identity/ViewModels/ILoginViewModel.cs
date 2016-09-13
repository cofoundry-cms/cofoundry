using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Cofoundry.Web.Identity
{
    public interface ILoginViewModel
    {
        string EmailAddress { get; set; }

        string Password { get; set; }

        bool RememberMe { get; set; }
    }
}