using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Domain
{
    /// <summary>
    /// Defines the Cofoundry admin panel user area.
    /// </summary>
    public class CofoundryAdminUserArea : IUserAreaDefinition
    {
        public CofoundryAdminUserArea(
            AdminSettings adminSetting
            )
        {
            LoginPath = "/" + adminSetting.DirectoryName + "/auth/login";
        }

        /// <summary>
        /// Constant containing the Cofoundry admin area UserAreaCode.
        /// </summary>
        public static string AreaCode = "COF";

        public string UserAreaCode { get; } = AreaCode;

        public string Name { get; } = "Cofoundry";

        public bool AllowPasswordLogin { get; } = true;

        public bool UseEmailAsUsername { get; } = true;

        public string LoginPath { get; private set;  }

        /// <summary>
        /// Although this is set to false, it is the fall-back schema if no default schema is set.
        /// </summary>
        public bool IsDefaultAuthSchema { get; } = false;
    }
}
