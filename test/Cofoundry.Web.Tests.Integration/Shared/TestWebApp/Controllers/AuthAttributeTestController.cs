using Cofoundry.Domain;
using Cofoundry.Domain.Tests.Integration;
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

        [Route("permission-on-anonymous-role")]
        [AuthorizePermission(typeof(ImageAssetReadPermission))]
        public IActionResult PermissionOnAnonymousRole()
        {
            return Ok();
        }

        [Route("permission")]
        [AuthorizePermission(typeof(PageUpdatePermission))]
        public IActionResult Permission()
        {
            return Ok();
        }

        [Route("custom-entity-permission")]
        [AuthorizePermission(typeof(CustomEntityCreatePermission), TestCustomEntityDefinition.Code)]
        public IActionResult CustomEntityPermission()
        {
            return Ok();
        }

        [Route("permission-non-default-user-area")]
        [AuthorizeUserArea(TestUserArea2.Code)]
        [AuthorizePermission(typeof(PageUpdatePermission))]
        public IActionResult PermissionForNonDefaultUserArea()
        {
            return Ok();
        }
    }
}
