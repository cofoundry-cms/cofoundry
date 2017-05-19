using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Cofoundry.BasicTestSite
{
    public class BasicTestSiteSettings : IConfigurationSettings
    {
        /// <summary>
        /// Setting Name = SimpleTestSite:ContactRequestNotificationToAddress
        /// </summary>
        [Required]
        [EmailAddress]
        public string ContactRequestNotificationToAddress { get; set; }
    }
}