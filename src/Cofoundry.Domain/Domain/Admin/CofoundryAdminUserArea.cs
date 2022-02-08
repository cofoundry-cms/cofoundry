using System;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Defines the Cofoundry admin panel user area.
    /// </summary>
    public class CofoundryAdminUserArea : IUserAreaDefinition
    {
        private readonly AdminSettings _adminSetting;

        public CofoundryAdminUserArea(
            AdminSettings adminSetting
            )
        {
            SignInPath = "/" + adminSetting.DirectoryName + "/auth/login";
            _adminSetting = adminSetting;
        }

        /// <summary>
        /// Constant containing the Cofoundry admin area UserAreaCode.
        /// </summary>
        public const string Code = "COF";

        [Obsolete("Renamed to 'Code' for consistency with other definitions.")]
        public const string AreaCode = Code;

        public string UserAreaCode { get; } = Code;

        public string Name { get; } = "Cofoundry";

        public bool AllowPasswordSignIn { get; } = true;

        public bool UseEmailAsUsername { get; } = true;

        public string SignInPath { get; private set; }

        /// <summary>
        /// Although this is set to false, it is the fall-back schema if no default schema is set.
        /// </summary>
        public bool IsDefaultAuthScheme { get; } = false;

        public void ConfigureOptions(UserAreaOptions options)
        {
            options.AccountRecovery.RecoveryUrlBase = "/" + _adminSetting.DirectoryName + "/auth/reset-password";
        }
    }
}
