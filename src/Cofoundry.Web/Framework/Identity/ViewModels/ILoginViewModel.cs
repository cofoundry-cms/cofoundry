using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web.Identity
{
    public interface ILoginViewModel
    {
        string Username { get; set; }

        string Password { get; set; }

        bool RememberMe { get; set; }
    }
}