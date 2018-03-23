using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web
{
    public class AuthorizeUserAreaAttribute : AuthorizeAttribute
    {
        public AuthorizeUserAreaAttribute(string userAreaCode)
            : base()
        {
            AuthenticationSchemes = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaCode);
        }
    }
}