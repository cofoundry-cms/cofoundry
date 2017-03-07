using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.DependencyInjection
{
    public class RegistrationOptions
    {
        public RegistrationOptions()
        {
            InstanceScope = InstanceScope.PerLifetimeScope;
        }

        /// <summary>
        /// Use this to override the existing implementation, e.g. for an 
        /// Azure module you could use this to override a file system service
        /// implementation with an azure blob service implementation
        /// </summary>
        public bool ReplaceExisting { get; set; }

        /// <summary>
        /// Use this to set a lifetime scope to the type. Defaults to PerLifetimeScope.
        /// </summary>
        public InstanceScope InstanceScope { get; set; }

        /// <summary>
        /// Use this value to set a priority level when ReplaceExisting is set to true. This
        /// should not normally be needed, but may be useful in overriding a particular 
        /// implementation. The RegistrationOverridePriority enum gives guidance and sensible defaults.
        /// </summary>
        public int RegistrationOverridePriority { get; set; }

        /// <summary>
        /// A quick way of creatinf a new options set which sets the
        /// ReplaceExisting property to true
        /// </summary>
        /// <param name="overridePriority">Optionally specify an override priority</param>
        public static RegistrationOptions Override(RegistrationOverridePriority? overridePriority = null)
        {
            var options = new RegistrationOptions();
            options.ReplaceExisting = true;

            if (overridePriority.HasValue)
            {
                options.RegistrationOverridePriority = (int)overridePriority;
            }

            return options;
        }
    }
}
