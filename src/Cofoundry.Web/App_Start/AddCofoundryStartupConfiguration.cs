﻿using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Localization;

namespace Cofoundry.Web
{
    /// <summary>
    /// Configuration class which can be used to customize the Cofoundry startup process. Most
    /// settings should be configured via the standard config files (which enabled transformations).
    /// </summary>
    public class AddCofoundryStartupConfiguration
    {
        public AddCofoundryStartupConfiguration()
        {
            AssemblyDiscoveryRules = new List<IAssemblyDiscoveryRule> { new CofoundryAssemblyDiscoveryRule() };
        }

        /// <summary>
        /// Add to or amend this collection to expand the assemblies used when
        /// scanning for types and registering dependencies. Teh default ruleset is
        /// defined in Cofoundry.Web.CofoundryAssemblyDiscoveryRule.
        /// </summary>
        public ICollection<IAssemblyDiscoveryRule> AssemblyDiscoveryRules { get; private set; }

        /// <summary>
        /// The default request culture 
        /// </summary>
        public RequestCulture DefaultRequestCulture { get; set; } = new RequestCulture("en");

        /// <summary>
        /// Enable localization in Cofoundry
        /// </summary>
        public bool EnableLocalization { get; set; } = false;
    }
}