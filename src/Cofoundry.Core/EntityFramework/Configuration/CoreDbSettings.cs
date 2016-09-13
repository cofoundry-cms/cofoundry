using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.EntityFramework
{
    public class CoreDbSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// The name of a connectionstring that can be used by modular packages to connect
        /// to the main database for an application.
        /// </summary>
        public string SharedConnectionStringName { get; set; }

        /// <summary>
        /// The name of a connectionstring that can be used by modular packages to connect
        /// to the main database for an application.
        /// </summary>
        public static string GetSharedConnectionStringName()
        {
            return ConfigurationHelper.GetSetting("CoreDb:SharedConnectionStringName");
        }
    }
}
