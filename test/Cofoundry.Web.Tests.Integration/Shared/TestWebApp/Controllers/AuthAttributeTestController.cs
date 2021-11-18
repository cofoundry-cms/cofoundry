using Cofoundry.Domain.Tests.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Cofoundry.Web.Tests.Integration
{
    [Route(RouteBase)]
    public class AuthAttributeTestController : ControllerBase
    {
        public const string RouteBase = "tests/auth-attributes";

        [Route("role")]
        [AuthorizeRole(TestUserArea2.Code, TestUserArea2RoleA.Code)]
        public IActionResult Role()
        {
            return Ok();
        }

        [Route("user-area")]
        [AuthorizeUserArea(TestUserArea2.Code)]
        public IActionResult UserArea()
        {
            return Ok();
        }
    }
}
