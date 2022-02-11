using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// The anonymous role is used for any unathenticated requests.
    /// Having an anonymous role allows you to configure permission
    /// for anyone not logged into the application.
    /// </summary>
    /// <inheritdoc/>
    public class AnonymousRole : IRoleDefinition
    {
        /// <summary>
        /// Constant value for the anonymous role code
        /// </summary>
        public const string Code = "ANO";

        [Obsolete("Renamed to 'Code' for consistency with other definitions.")]
        public const string AnonymousRoleCode = "ANO";

        public string Title { get { return "Anonymous"; } }

        public string RoleCode { get { return Code; } }

        public string UserAreaCode { get { return CofoundryAdminUserArea.Code; } }
    }
}
