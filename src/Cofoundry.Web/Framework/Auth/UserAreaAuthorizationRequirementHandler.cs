using Microsoft.AspNetCore.Authorization;

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