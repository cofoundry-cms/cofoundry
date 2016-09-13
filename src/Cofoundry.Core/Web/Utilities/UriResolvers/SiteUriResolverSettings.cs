using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Cofoundry.Core.Configuration;

namespace Cofoundry.Core.Web
{
    public class SiteUriResolverSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// The root uri path to use when resolving a uri, e.g. 'http://www.cofoundry.org'
        /// </summary>
        [Required]
        public string SiteUriRoot { get; set; }
    }
}
