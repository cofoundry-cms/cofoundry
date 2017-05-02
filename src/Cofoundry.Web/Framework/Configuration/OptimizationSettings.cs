using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Web
{
    public class OptimizationSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// By default css/js is only bundles in release mode, set this to true to override
        /// this behavior (only applicable when using the asp.net bundling system).
        /// </summary>
        public bool ForceBundling { get; set; }
    }
}
