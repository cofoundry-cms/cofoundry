using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core.Configuration
{
    public class CofoundryConfigurationSettingsBase : INamespacedConfigurationSettings
    {
        public string Namespace
        {
            get
            {
                return "Cofoundry";
            }
        }
    }
}
