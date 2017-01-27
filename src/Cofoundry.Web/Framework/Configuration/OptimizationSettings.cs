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
        /// Indicates whether to remove to remove whitespace from the html output
        /// or not. Defaults to false.
        /// </summary>
        public bool RemoveWhitespaceFromHtml { get; set; }

        /// <summary>
        /// By default css/js is only bundles in release mode, set this to true to override
        /// this behavior (only applicable when using the asp.net bundling system).
        /// </summary>
        public bool ForceBundling { get; set; }
    }
}
