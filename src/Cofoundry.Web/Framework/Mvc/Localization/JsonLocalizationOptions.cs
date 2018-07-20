using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Microsoft.Extensions.Localization;

namespace Cofoundry.Web.Framework.Mvc.Localization
{
    /// <summary>
    /// Options class for JSON localization.
    /// </summary>
    public class JsonLocalizationOptions : LocalizationOptions
    {
        /// <summary>
        /// The default culture.
        /// </summary>
        public CultureInfo DefaultCulture { get; set; } = new CultureInfo("en");

        /// <summary>
        /// Assemsblies to check for emnedded resrources
        /// </summary>
        public List<Assembly> Assemblies { get; set; } = new List<Assembly>();
    }
}