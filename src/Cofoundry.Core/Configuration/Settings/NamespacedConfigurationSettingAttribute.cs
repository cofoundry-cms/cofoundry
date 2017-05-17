using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Configuration
{
    /// <summary>
    /// An attribute that can be applied to a settings class that allows
    /// it to be nested at a deeper namespace level. E.g. Cofoundry uses this to
    /// put settings inside a "Cofoundry" grouping container.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class NamespacedConfigurationSettingAttribute : Attribute
    {
        /// <summary>
        /// An attribute that can be applied to a settings class that allows
        /// it to be nested at a deeper namespace level. E.g. Cofoundry uses this to
        /// put settings inside a "Cofoundry" grouping container.
        /// </summary>
        /// <param name="settingNamespace">
        /// The namespace or outer grouping level name to wrap the configuration
        /// settings in. This can be a single group name e.g. "Cofoundry" or multiple 
        /// levels delmited by a colon, e.g. "Cofoundry:Plugins"
        /// </param>
        public NamespacedConfigurationSettingAttribute(string settingNamespace)
        {
            Namespace = settingNamespace;
        }

        /// <summary>
        /// The namespace or outer grouping level name to wrap the configuration
        /// settings in. This can be a single group name e.g. "Cofoundry" or multiple 
        /// levels delmited by a colon, e.g. "Cofoundry:Plugins"
        /// </summary>
        public string Namespace { get; private set; }
    }
}
