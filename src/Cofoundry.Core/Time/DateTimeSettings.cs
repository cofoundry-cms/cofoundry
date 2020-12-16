using Cofoundry.Core.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cofoundry.Core.Time
{
    /// <summary>
    /// Settings used by the DatetimeService.
    /// </summary>
    public class DateTimeSettings : CofoundryConfigurationSettingsBase
    {
        /// <summary>
        /// The base date can be used when testing or debugging to alter the 
        /// date and time used by Cofoundry as "Now". When the application
        /// starts up the "Now" value used by the DateTimeService is set this
        /// value, and continues to tick as normal. This can useful for testing
        /// promotions that occur at a set date and time.
        /// </summary>
        public DateTime? BaseDate { get; set; }
    }
}
