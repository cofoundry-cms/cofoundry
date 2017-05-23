using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Cofoundry.Domain;
using Microsoft.AspNetCore.Authorization;

namespace Cofoundry.Web.Admin
{
    public class AuthorizeUserAreaAttribute : AuthorizeAttribute
    {
        public AuthorizeUserAreaAttribute(string userAreaCode)
            : base(CofoundryAuthenticationConstants.FormatPolicyName(userAreaCode))
        {
            ActiveAuthenticationSchemes = CofoundryAuthenticationConstants.FormatAuthenticationScheme(userAreaCode);
        }
    }
}