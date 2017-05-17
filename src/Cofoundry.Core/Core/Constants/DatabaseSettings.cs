using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cofoundry.Core
{
    /// <summary>
    /// Settings to use when connecting to the Cofoundry database.
    /// </summary>
    public class DatabaseSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// The connection string to the Cofoundry database.
        /// </summary>
        [Required]
        public string ConnectionString { get; set; }
    }
}
