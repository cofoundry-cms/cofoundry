using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cofoundry.Web
{
    public class UserAreaAuthorizationRequirement : IAuthorizationRequirement
    {
        public UserAreaAuthorizationRequirement(
            string userAreaCode
            )
        {
            UserAreaCode = userAreaCode;
        }

        public string UserAreaCode { get; private set; }
    }
}