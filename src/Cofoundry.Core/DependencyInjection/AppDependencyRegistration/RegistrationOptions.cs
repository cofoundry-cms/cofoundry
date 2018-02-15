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
            Lifetime = InstanceLifetime.Transient;
        }

        /// <summary>
        /// Use this to override the existing implementation, e.g. for an 
        /// Azure module you could use this to override a file system service
        /// implementation with an azure blob service implementation
        /// </summary>
        public bool ReplaceExisting { get; set; }

        /// <summary>
        /// Use this to set a lifetime to the type. Defaults to Transient.
        /// </summary>
        public InstanceLifetime Lifetime { get; set; }

        /// <summary>
        /// Use this value to set a priority level when ReplaceExisting is set to true. This
        /// should not normally be needed, but may be useful in overriding a particular 
        /// implementation. The RegistrationOverridePriority enum gives guidance and sensible defaults.
        /// </summary>
        public int RegistrationOverridePriority { get; set; }

        /// <summary>
        /// A quick way of creating a new options set which sets the
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

        /// <summary>
        /// A quick way of creating a new options set which sets the
        /// Lifetime property to InstanceLifetime.Singleton
        /// </summary>
        public static RegistrationOptions SingletonScope()
        {
            return new RegistrationOptions() { Lifetime = InstanceLifetime.Singleton };
        }

        /// <summary>
        /// A quick way of creating a new options set which sets the
        /// Lifetime property to InstanceLifetime.Transient
        /// </summary>
        public static RegistrationOptions TransientScope()
        {
            return new RegistrationOptions() { Lifetime = InstanceLifetime.Transient };
        }

        /// <summary>
        /// A quick way of creating a new options set which sets the
        /// Lifetime property to InstanceLifetime.Scoped
        /// </summary>
        public static RegistrationOptions Scoped()
        {
            return new RegistrationOptions() { Lifetime = InstanceLifetime.Scoped };
        }
    }
}
